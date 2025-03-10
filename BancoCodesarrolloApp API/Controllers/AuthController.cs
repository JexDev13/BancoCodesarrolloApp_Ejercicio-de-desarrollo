using AutoMapper;
using BancoCodesarrolloApp_API.Context;
using BancoCodesarrolloApp_API.DTO.Usuario;
using BancoCodesarrolloApp_API.Models;
using BancoCodesarrolloApp_API.Services.Token;
using BancoCodesarrolloApp_API.Utils.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoCodesarrolloApp_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(ActionFilterToken))]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IToken _token;
        private readonly BankContext _context;

        public AuthController(IConfiguration configuration, IToken token, BankContext context, IMapper mapper)
        {
            _configuration = configuration;
            _token = token;
            _context = context;
            _mapper = mapper;
        }

        // Método para crear un usuario
        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UsuarioCreacionDTO usuarioDTO)
        {
            var clienteExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Identificacion == usuarioDTO.Identificacion);

            if (clienteExistente != null) return BadRequest("Cliente ya registrado.");

            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == usuarioDTO.CorreoElectronico);

            if (usuarioExistente != null) return BadRequest("Ya existe un usuario con el correo ingresado.");

            var cliente = new Usuario
            {
                Nombre = usuarioDTO.Nombre,
                Apellido = usuarioDTO.Apellido,
                CorreoElectronico = usuarioDTO.CorreoElectronico,
                Identificacion = usuarioDTO.Identificacion,
                Direccion = usuarioDTO.Direccion,
                Contraseña = new PasswordHasher<Usuario>().HashPassword(null, usuarioDTO.Contraseña),
                Estado = 1
            };

            _context.Usuarios.Add(cliente);
            await _context.SaveChangesAsync();

            var userResponseDTO = _mapper.Map<UsuarioConsultaDTO>(cliente);

            return Ok(userResponseDTO);
        }

        // Método para iniciar sesión
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserCredentials model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Correo y contraseña son obligatorios.");

            string correoNormalizado = model.Email.Trim().ToLower();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.CorreoElectronico == correoNormalizado);

            if (usuario == null || usuario.Estado == 0)
                return BadRequest("Usuario o contraseña incorrectos.");

            var passwordHasher = new PasswordHasher<Usuario>();
            var verificationResult = passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, model.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                return BadRequest("Usuario o contraseña incorrectos.");

            var token = JwtToken.BuildToken(model, _configuration["JWT:key"], usuario.Id);
            token.Nombre = $"{usuario.Nombre} {usuario.Apellido}";
            token.Email = usuario.CorreoElectronico;
            token.UsuarioId = usuario.Id;

            await AdministrarSesion(usuario.Id, token);

            return Ok(token);
        }

        //Método para cerrar sesión
        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            int usuarioId = _token.ObtenerUsuarioId(HttpContext.User.Claims);

            var sesion = await _context.Sessions.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

            if (sesion == null)
                return NotFound("Sesión no encontrada o ya cerrada.");

            _context.Sessions.Remove(sesion);
            await _context.SaveChangesAsync();

            return Ok("Sesión cerrada correctamente.");
        }

        private async Task AdministrarSesion(int usuarioId, UserToken token)
        {
            var sesionExistente = await _context.Sessions.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

            if (sesionExistente != null)
                _context.Sessions.Remove(sesionExistente);

            _context.Sessions.Add(new Session { Token = token.Token, UsuarioId = usuarioId });
            await _context.SaveChangesAsync();
        }
    }
}
