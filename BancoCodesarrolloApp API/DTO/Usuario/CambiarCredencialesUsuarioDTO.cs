namespace BancoCodesarrolloApp_API.DTO.Usuario
{
    public class CambiarCredencialesUsuarioDTO
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
