using Microsoft.EntityFrameworkCore;
using Transporte.Model;

namespace Transporte.DA;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chofer> Chofer { get; set; }
    public virtual DbSet<Pasajero> Pasajero { get; set; }
    public virtual DbSet<Reserva> Reserva { get; set; }
    public virtual DbSet<Ruta> Ruta { get; set; }
    public virtual DbSet<Unidad> Unidad { get; set; }
    public virtual DbSet<Usuario> Usuario { get; set; }
    public virtual DbSet<Viaje> Viaje { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // La configuración viene de Program.cs / appsettings.json
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chofer>(entity =>
        {
            entity.ToTable("Choferes");
            entity.HasKey(e => e.Cedula).HasName("PK__Choferes__415B7BE4B0FEE9F9");

            entity.Property(e => e.Cedula)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido2");
            entity.Property(e => e.Nombre1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre1");
            entity.Property(e => e.Nombre2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre2");

            entity.HasOne(d => d.CedulaNavigation).WithOne(p => p.Chofere)
                .HasForeignKey<Chofer>(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Choferes_Usuarios");
        });

        modelBuilder.Entity<Pasajero>(entity =>
        {
            entity.ToTable("Pasajeros");
            entity.HasKey(e => e.Cedula).HasName("PK__Pasajero__415B7BE48A5B7A6C");

            entity.Property(e => e.Cedula)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido2");
            entity.Property(e => e.Nombre1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre1");
            entity.Property(e => e.Nombre2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre2");

            entity.HasOne(d => d.CedulaNavigation).WithOne(p => p.Pasajero)
                .HasForeignKey<Pasajero>(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pasajeros_Usuarios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Cedula).HasName("PK__Usuarios__415B7BE4EC1B5115");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__2A586E0B88573A0B").IsUnique();
            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuarios__D4D22D74472290D1").IsUnique();

            entity.Property(e => e.Cedula)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.Bloqueado).HasColumnName("bloqueado");
            entity.Property(e => e.Clave)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IntentosFallidos).HasColumnName("intentos_fallidos");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.TipoUsuario)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo_usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}