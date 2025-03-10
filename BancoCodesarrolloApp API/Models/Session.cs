namespace BancoCodesarrolloApp_API.Models
{
    public partial class Session
    {
        public int SesionId { get; set; }
        public int? UsuarioId { get; set; }
        public string? Token { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
