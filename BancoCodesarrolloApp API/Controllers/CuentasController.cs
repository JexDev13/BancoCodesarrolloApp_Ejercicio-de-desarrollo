using AutoMapper;
using BancoCodesarrolloApp_API.Context;
using BancoCodesarrolloApp_API.DTO.Cuenta;
using BancoCodesarrolloApp_API.Models;
using BancoCodesarrolloApp_API.Services.Token;
using BancoCodesarrolloApp_API.Utils.GeneradorCuentas;
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
    public class CuentasController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly IMapper _mapper;
        private readonly IToken _token;

        public CuentasController(BankContext context, IMapper mapper, IToken token)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
        }

        //Método para crear una cuenta
        [HttpPost(Name = "CrearCuenta")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<CuentaConsultaDTO>> CrearCuenta([FromBody] CuentaCreacionDTO nuevaCuenta)
        {
            var usuario = await _context.Usuarios.FindAsync(nuevaCuenta.UsuarioId);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            var cuenta = new Cuenta
            {
                NumeroCuenta = GeneradorCuentas.GenerarNumeroCuenta(nuevaCuenta.UsuarioId),
                TipoCuenta = nuevaCuenta.TipoCuenta,
                Saldo = 0,
                Estado = 1,
                UsuarioId = nuevaCuenta.UsuarioId
            };

            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            var cuentaDto = _mapper.Map<CuentaConsultaDTO>(cuenta);
            return CreatedAtRoute("GetCuenta", new { id = cuenta.Id }, cuentaDto);
        }

        //Método para obtener la cuenta por id
        [HttpGet("{id}", Name = "GetCuenta")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<CuentaConsultaDTO>> GetCuenta(int id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null) return NotFound(new { mensaje = "Cuenta no encontrada" });
            var cuentaDto = _mapper.Map<CuentaConsultaDTO>(cuenta);
            return Ok(cuentaDto);
        }

        //Método para obtener la cuenta por Número de cuenta
        [HttpGet("numero/{numeroCuenta}", Name = "GetCuentaPorNumero")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<CuentaConsultaDTO>> GetCuentaPorNumero(string numeroCuenta)
        {
            var cuenta = await _context.Cuentas
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

            if (cuenta == null) return NotFound(new { mensaje = "Cuenta no encontrada" });

            var cuentaDto = _mapper.Map<CuentaConsultaDTO>(cuenta);
            return Ok(cuentaDto);
        }

        //Método para obtener las cuentas de un usuario
        [HttpGet("usuario/{id}", Name = "GetCuentasUsuario")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<IEnumerable<CuentaConsultaDTO>>> GetCuentasUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Cuentas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            if (usuario.Cuentas == null || !usuario.Cuentas.Any())
                return NotFound(new { mensaje = "El usuario no tiene cuentas registradas" });

            var cuentasDto = _mapper.Map<IEnumerable<CuentaConsultaDTO>>(usuario.Cuentas);
            return Ok(cuentasDto);
        }

        //Método para actualizar el saldo
        [HttpPatch("{id}", Name = "ActualizarSaldo")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<IActionResult> ActualizarSaldo(int id, [FromBody] CuentaActualizacionDTO cuentaDto)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null) return NotFound(new { mensaje = "Cuenta no encontrada" });
            cuenta.Saldo = cuentaDto.Saldo;
            _context.Cuentas.Update(cuenta);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //Método para desactivar/activar la cuenta
        [HttpPatch("estado/{id}", Name = "ActivarDesactivarCuenta")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<IActionResult> DesactivarCuenta(int id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null) return NotFound(new { mensaje = "Cuenta no encontrada" });

            cuenta.Estado = cuenta.Estado == 1 ? 0 : 1;
            _context.Cuentas.Update(cuenta);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Cuenta {(cuenta.Estado == 1 ? "activado" : "desactivado")}" });
        }

        //Método para eliminar la cuenta
        [HttpDelete("{id}", Name = "EliminarCuenta")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<IActionResult> EliminarCuenta(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cuenta = await _context.Cuentas
                    .Include(c => c.Movimientos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cuenta == null) return NotFound(new { mensaje = "Cuenta no encontrada" });

                if (cuenta.Movimientos.Any())
                {
                    _context.Movimientos.RemoveRange(cuenta.Movimientos);
                    await _context.SaveChangesAsync();
                }

                _context.Cuentas.Remove(cuenta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new { mensaje = "Cuenta eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error interno al eliminar la cuenta", error = ex.Message });
            }
        }
    }
}
