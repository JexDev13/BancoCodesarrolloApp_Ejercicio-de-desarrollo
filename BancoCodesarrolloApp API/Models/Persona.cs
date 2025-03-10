namespace BancoCodesarrolloApp_API.Models
{
    public partial class Persona
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Identificacion { get; set; }
        public string? Direccion { get; set; }
        public string? CorreoElectronico { get; set; }
    }
}
