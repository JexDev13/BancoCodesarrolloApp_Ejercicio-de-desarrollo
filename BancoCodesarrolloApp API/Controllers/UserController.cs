using AutoMapper;
using BancoCodesarrolloApp_API.Context;
using BancoCodesarrolloApp_API.DTO.Usuario;
using BancoCodesarrolloApp_API.Services.Token;
using BancoCodesarrolloApp_API.Utils.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoCodesarrolloApp_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly IMapper _mapper;
        private readonly IToken _token;

        public UserController(BankContext context, IMapper mapper, IToken token)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
        }

        //Método para obtener el usuario
        [HttpGet("{id}", Name = "GetUsuario")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<UsuarioConsultaDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            var usuarioDto = _mapper.Map<UsuarioConsultaDTO>(usuario);
            return Ok(usuarioDto);
        }

        //Método para actualizar el usuario
        [HttpPatch("{id}", Name = "ActualizarUsuario")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioActualizacionDTO usuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            usuario.Nombre = usuarioDto.Nombre;
            usuario.Apellido = usuarioDto.Apellido;
            usuario.Direccion = usuarioDto.Direccion;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Método para desactivar/activar el usuario
        [HttpPatch("estado/{id}", Name = "ActivarDesactivarUsuario")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<IActionResult> ActivarDesactivarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            usuario.Estado = usuario.Estado == 1 ? 0 : 1;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Usuario {(usuario.Estado == 1 ? "activado" : "desactivado")}" });
        }

        //Método para eliminar el usuario
        [HttpDelete("{id}", Name = "EliminarUsuario")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Cuentas)
                    .ThenInclude(c => c.Movimientos)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

                var movimientos = usuario.Cuentas.SelectMany(c => c.Movimientos).ToList();
                if (movimientos.Any())
                {
                    _context.Movimientos.RemoveRange(movimientos);
                    await _context.SaveChangesAsync();
                }

                if (usuario.Cuentas.Any())
                {
                    _context.Cuentas.RemoveRange(usuario.Cuentas);
                    await _context.SaveChangesAsync();
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                var persona = await _context.Personas.FindAsync(id);
                if (persona != null)
                {
                    _context.Personas.Remove(persona);
                    await _context.SaveChangesAsync();
                }

                var sesion = await _context.Sessions.FirstOrDefaultAsync(s => s.UsuarioId == id);
                if (sesion != null)
                {
                    _context.Sessions.Remove(sesion);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { mensaje = "Usuario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error interno al eliminar el usuario", error = ex.Message });
            }
        }
    }
}
