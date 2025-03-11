using System.ComponentModel.DataAnnotations;

namespace BancoCodesarrolloApp_API.DTO.Movimiento
{
    public class MovimientoCreacionDTO
    {
        [Required]
        public string TipoMovimiento { get; set; } = string.Empty;
        [Required]
        public decimal Valor { get; set; }
        [Required]
        public int CuentaId { get; set; }
    }
}
