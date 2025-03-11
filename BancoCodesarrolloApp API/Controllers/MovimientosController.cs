using AutoMapper;
using BancoCodesarrolloApp_API.Context;
using BancoCodesarrolloApp_API.DTO.Movimiento;
using BancoCodesarrolloApp_API.Models;
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
    public class MovimientosController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly IMapper _mapper;
        private readonly IToken _token;

        public MovimientosController(BankContext context, IMapper mapper, IToken token)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
        }

        //Método para crear un movimiento
        [HttpPost(Name = "CrearMovimiento")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<MovimientoConsultaDTO>> CrearMovimiento([FromBody] MovimientoCreacionDTO nuevoMovimiento)
        {
            var cuenta = await _context.Cuentas
                .FirstOrDefaultAsync(c => c.Id == nuevoMovimiento.CuentaId);

            if (cuenta == null) return NotFound(new { mensaje = "Cuenta no encontrada" });

            if (nuevoMovimiento.TipoMovimiento.ToLower() == "debito")
            {
                if (cuenta.Saldo < nuevoMovimiento.Valor)
                    return BadRequest(new { mensaje = "Saldo insuficiente" });

                cuenta.Saldo -= nuevoMovimiento.Valor;
            }
            else if (nuevoMovimiento.TipoMovimiento.ToLower() == "credito")
            {
                cuenta.Saldo += nuevoMovimiento.Valor;
            }
            else
            {
                return BadRequest(new { mensaje = "Tipo de movimiento inválido" });
            }

            var movimiento = new Movimiento
            {
                Fecha = DateTime.UtcNow,
                TipoMovimiento = nuevoMovimiento.TipoMovimiento,
                Valor = nuevoMovimiento.Valor,
                Saldo = cuenta.Saldo,
                CuentaId = cuenta.Id
            };

            _context.Movimientos.Add(movimiento);
            _context.Cuentas.Update(cuenta);
            await _context.SaveChangesAsync();

            var movimientoDto = _mapper.Map<MovimientoConsultaDTO>(movimiento);
            return CreatedAtRoute("GetMovimiento", new { id = movimiento.Id }, movimientoDto);
        }


        //Método para obtener los movimientos de una cuenta
        [HttpGet("cuenta/{id}", Name = "GetMovimientosPorCuenta")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<IEnumerable<MovimientoConsultaDTO>>> GetMovimientosPorCuenta(int id)
        {
            var movimientos = await _context.Movimientos
                .Where(m => m.CuentaId == id)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            if (!movimientos.Any())
                return NotFound(new { mensaje = "No hay movimientos para esta cuenta" });

            var movimientosDto = _mapper.Map<IEnumerable<MovimientoConsultaDTO>>(movimientos);
            return Ok(movimientosDto);
        }

        //Método para obtener un movimiento por id
        [HttpGet("{id}", Name = "GetMovimiento")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<MovimientoConsultaDTO>> GetMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null) return NotFound(new { mensaje = "Movimiento no encontrado" });

            var movimientoDto = _mapper.Map<MovimientoConsultaDTO>(movimiento);
            return Ok(movimientoDto);
        }

        //Método para obtener los movimientos de un usuario
        [HttpGet("usuario/{id}", Name = "GetMovimientosPorUsuario")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<ActionResult<IEnumerable<MovimientoConsultaDTO>>> GetMovimientosPorUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Cuentas)
                .ThenInclude(c => c.Movimientos)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            var movimientos = usuario.Cuentas?
                .SelectMany(c => c.Movimientos)
                .OrderByDescending(m => m.Fecha)
                .ToList();

            if (movimientos == null || !movimientos.Any())
                return NotFound(new { mensaje = "No hay movimientos para este usuario" });

            var movimientosDto = _mapper.Map<IEnumerable<MovimientoConsultaDTO>>(movimientos);
            return Ok(movimientosDto);
        }

        //Método para eliminar un movimiento
        [HttpDelete("{id}", Name = "EliminarMovimiento")]
        [ServiceFilter(typeof(ActionFilterToken))]
        public async Task<IActionResult> EliminarMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null) return NotFound(new { mensaje = "Movimiento no encontrado" });

            var cuenta = await _context.Cuentas.FindAsync(movimiento.CuentaId);
            if (cuenta == null) return NotFound(new { mensaje = "Cuenta asociada no encontrada" });

            // Revertir el saldo en la cuenta
            if (movimiento.TipoMovimiento.ToLower() == "debito")
            {
                cuenta.Saldo += movimiento.Valor;
            }
            else if (movimiento.TipoMovimiento.ToLower() == "credito")
            {
                cuenta.Saldo -= movimiento.Valor;
            }

            _context.Movimientos.Remove(movimiento);
            _context.Cuentas.Update(cuenta);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Movimiento eliminado exitosamente" });
        }
    }
}
