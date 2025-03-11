using BancoCodesarrolloApp_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoCodesarrolloApp_API.Context
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Persona>()
            .HasIndex(p => p.Identificacion)
            .IsUnique();

            modelBuilder.Entity<Persona>()
            .HasIndex(p => p.CorreoElectronico)
            .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasOne<Persona>()
                .WithOne()
                .HasForeignKey<Usuario>(u => u.Id);

            modelBuilder.Entity<Cuenta>()
            .HasIndex(p => p.NumeroCuenta)
            .IsUnique();

            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Cuentas)
                .HasForeignKey(c => c.UsuarioId);

            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.Cuenta)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.CuentaId);

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.SesionId);
                entity.Property(e => e.SesionId).HasColumnName("Sesion_Id");

                entity.Property(e => e.Token)
                    .IsUnicode(false)
                    .HasColumnName("Token");

                entity.Property(e => e.UsuarioId).HasColumnName("Usuario_Id");
            });

            modelBuilder.Entity<Persona>().ToTable("Personas");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cuenta>().ToTable("Cuentas");
            modelBuilder.Entity<Movimiento>().ToTable("Movimientos");
            modelBuilder.Entity<Session>().ToTable("Sessions");
        }
    }
}
