using System.ComponentModel.DataAnnotations;

namespace BancoCodesarrolloApp_API.DTO.Usuario
{
    public class UsuarioCreacionDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        public string CorreoElectronico { get; set; } = string.Empty;
        [Required]
        public string Identificacion { get; set; } = string.Empty;
        [Required]
        public string Direccion { get; set; } = string.Empty;
        [Required]
        public string Contraseña { get; set; } = string.Empty;
    }
}
