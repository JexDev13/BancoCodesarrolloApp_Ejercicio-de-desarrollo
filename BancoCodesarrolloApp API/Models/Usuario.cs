namespace BancoCodesarrolloApp_API.Models
{
    public partial class Usuario : Persona
    {
        public string? Contraseña { get; set; }
        public int Estado { get; set; }
        public ICollection<Cuenta>? Cuentas { get; set; }
        public Session? Session { get; set; }
    }
}
