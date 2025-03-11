using System.ComponentModel.DataAnnotations;

namespace BancoCodesarrolloApp_API.DTO.Cuenta
{
    public class CuentaCreacionDTO
    {
        [Required]
        public string TipoCuenta { get; set; } = string.Empty;
        [Required]
        public int UsuarioId { get; set; }
    }
}
