namespace BancoCodesarrolloApp_API.Models
{
    public partial class Cuenta
    {
        public int Id { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? TipoCuenta { get; set; }
        public decimal Saldo { get; set; }
        public int Estado { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<Movimiento>? Movimientos { get; set; }
    }
}
