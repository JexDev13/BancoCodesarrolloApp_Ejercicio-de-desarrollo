namespace BancoCodesarrolloApp_API.DTO.Usuario
{
    public class UserToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
