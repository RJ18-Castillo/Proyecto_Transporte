using Microsoft.EntityFrameworkCore;
using Transporte.Model;

namespace Transporte.DA
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){ }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Chofer> Choferes { get; set; }
        public DbSet<Pasajero> Pasajeros { get; set; }
        public DbSet<Ruta> Rutas { get; set; }
        public DbSet<Unidad> Unidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(usuario => usuario.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(usuario => usuario.Correo)
                .IsUnique();

            modelBuilder.Entity<Unidad>()
                .HasIndex(unidad => unidad.Placa)
                .IsUnique();

            modelBuilder.Entity<Pasajero>()
                .HasOne(pasajero => pasajero.Usuario)
                .WithMany()
                .HasForeignKey(pasajero => pasajero.UsuarioId);

            base.OnModelCreating(modelBuilder);
        }
    }
}