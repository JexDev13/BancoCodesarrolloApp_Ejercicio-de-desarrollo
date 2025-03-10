using System.ComponentModel.DataAnnotations;

namespace BancoCodesarrolloApp_API.DTO.Cuenta
{
    public class CuentaCreacionDTO
    {
        [Required]
        public string NumeroCuenta { get; set; } = string.Empty;
        [Required]
        public string TipoCuenta { get; set; } = string.Empty;
        [Required]
        public decimal Saldo { get; set; }
        [Required]
        public string Estado { get; set; } = string.Empty;
        [Required]
        public int UsuarioId { get; set; }
    }
}
