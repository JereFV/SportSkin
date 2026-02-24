using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Models;

namespace SportSkin.Infrastructure.Data;

public partial class SportSkinContext : DbContext
{
    public SportSkinContext(DbContextOptions<SportSkinContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Camiseta> Camiseta { get; set; }

    public virtual DbSet<CategoriaCamiseta> CategoriaCamiseta { get; set; }

    public virtual DbSet<CondicionCamiseta> CondicionCamiseta { get; set; }

    public virtual DbSet<DatosEnvio> DatosEnvio { get; set; }

    public virtual DbSet<Equipo> Equipo { get; set; }

    public virtual DbSet<EstadoCamiseta> EstadoCamiseta { get; set; }

    public virtual DbSet<EstadoFactura> EstadoFactura { get; set; }

    public virtual DbSet<EstadoSubasta> EstadoSubasta { get; set; }

    public virtual DbSet<Factura> Factura { get; set; }

    public virtual DbSet<ImagenCamiseta> ImagenCamiseta { get; set; }

    public virtual DbSet<Jugador> Jugador { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Pais> Pais { get; set; }

    public virtual DbSet<ParametroSubasta> ParametroSubasta { get; set; }

    public virtual DbSet<Puja> Puja { get; set; }

    public virtual DbSet<RolUsuario> RolUsuario { get; set; }

    public virtual DbSet<Subasta> Subasta { get; set; }

    public virtual DbSet<TrayectoriaJugadorEquipo> TrayectoriaJugadorEquipo { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<ZonaEnvio> ZonaEnvio { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Camiseta>(entity =>
        {
            entity.HasKey(e => e.IdCamiseta);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EstadoRegistro).HasDefaultValue(true);
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Camiseta)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Camiseta_CategoriaCamiseta");

            entity.HasOne(d => d.IdCondicionCamisetaNavigation).WithMany(p => p.Camiseta)
                .HasForeignKey(d => d.IdCondicionCamiseta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Camiseta_CondicionCamiseta");

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.Camiseta)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Camiseta_Equipo");

            entity.HasOne(d => d.IdEstadoCamisetaNavigation).WithMany(p => p.Camiseta)
                .HasForeignKey(d => d.IdEstadoCamiseta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Camiseta_EstadoCamiseta");

            entity.HasOne(d => d.IdJugadorNavigation).WithMany(p => p.Camiseta)
                .HasForeignKey(d => d.IdJugador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Camiseta_Jugador");

            entity.HasOne(d => d.IdUsuarioVendedorNavigation).WithMany(p => p.Camiseta)
                .HasForeignKey(d => d.IdUsuarioVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Camiseta_Usuario");
        });

        modelBuilder.Entity<CategoriaCamiseta>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaCamiseta);

            entity.Property(e => e.Nombre)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CondicionCamiseta>(entity =>
        {
            entity.HasKey(e => e.IdCondicionCamiseta);

            entity.Property(e => e.Nombre)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DatosEnvio>(entity =>
        {
            entity.HasKey(e => e.IdDatosEnvio);

            entity.Property(e => e.IdDatosEnvio).ValueGeneratedNever();
            entity.Property(e => e.Ciudad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DireccionExacta)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Region)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPaisNavigation).WithMany(p => p.DatosEnvio)
                .HasForeignKey(d => d.IdPais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatosEnvio_Pais");

            entity.HasOne(d => d.IdSubastaNavigation).WithMany(p => p.DatosEnvio)
                .HasForeignKey(d => d.IdSubasta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatosEnvio_Subasta");
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasKey(e => e.IdEquipo);

            entity.HasIndex(e => e.IdExternoEquipo, "UK_idExternalEquipo").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Pais)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoCamiseta>(entity =>
        {
            entity.HasKey(e => e.IdEstadoCamiseta).HasName("PK__EstadoCa__5F720719193A2124");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoFactura>(entity =>
        {
            entity.HasKey(e => e.IdEstadoFactura);

            entity.Property(e => e.Nombre)
                .HasMaxLength(12)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoSubasta>(entity =>
        {
            entity.HasKey(e => e.IdEstadoSubasta);

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura);

            entity.Property(e => e.IdFactura)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");

            entity.HasOne(d => d.IdEstadoFacturaNavigation).WithMany(p => p.Factura)
                .HasForeignKey(d => d.IdEstadoFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_EstadoFactura");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Factura)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_MetodoPago");

            entity.HasOne(d => d.IdSubastaNavigation).WithMany(p => p.Factura)
                .HasForeignKey(d => d.IdSubasta)
                .HasConstraintName("FK_Factura_Subasta");
        });

        modelBuilder.Entity<ImagenCamiseta>(entity =>
        {
            entity.HasKey(e => new { e.IdImagen, e.IdCamiseta });

            entity.Property(e => e.RutaImagen)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCamisetaNavigation).WithMany(p => p.ImagenCamiseta)
                .HasForeignKey(d => d.IdCamiseta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImagenCamiseta_Camiseta");
        });

        modelBuilder.Entity<Jugador>(entity =>
        {
            entity.HasKey(e => e.IdJugador);

            entity.HasIndex(e => e.IdExternoJugador, "UK_idExternalJugador").IsUnique();

            entity.Property(e => e.Apellido)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Nacionalidad)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago);

            entity.Property(e => e.Nombre)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Pais>(entity =>
        {
            entity.HasKey(e => e.IdPais);

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdZonaEnvioNavigation).WithMany(p => p.Pais)
                .HasForeignKey(d => d.IdZonaEnvio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pais_ZonaEnvio");
        });

        modelBuilder.Entity<ParametroSubasta>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Puja>(entity =>
        {
            entity.HasKey(e => new { e.IdPuja, e.IdSubasta });

            entity.Property(e => e.Fecha).HasColumnType("datetime");

            entity.HasOne(d => d.IdSubastaNavigation).WithMany(p => p.Puja)
                .HasForeignKey(d => d.IdSubasta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puja_Subasta");

            entity.HasOne(d => d.IdUsuarioPujaNavigation).WithMany(p => p.Puja)
                .HasForeignKey(d => d.IdUsuarioPuja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puja_Usuario");
        });

        modelBuilder.Entity<RolUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRolUsuario);

            entity.Property(e => e.IdRolUsuario).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Subasta>(entity =>
        {
            entity.HasKey(e => e.IdSubasta);

            entity.Property(e => e.IdSubasta).ValueGeneratedNever();
            entity.Property(e => e.FechaCierre).HasColumnType("datetime");
            entity.Property(e => e.FechaCompra).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");

            entity.HasOne(d => d.IdCamisetaNavigation).WithMany(p => p.Subasta)
                .HasForeignKey(d => d.IdCamiseta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subasta_Camiseta");

            entity.HasOne(d => d.IdEstadoSubastaNavigation).WithMany(p => p.Subasta)
                .HasForeignKey(d => d.IdEstadoSubasta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subasta_EstadoSubasta");

            entity.HasOne(d => d.IdUsuarioCompradorNavigation).WithMany(p => p.Subasta)
                .HasForeignKey(d => d.IdUsuarioComprador)
                .HasConstraintName("FK_Subasta_Usuario");
        });

        modelBuilder.Entity<TrayectoriaJugadorEquipo>(entity =>
        {
            entity.HasKey(e => new { e.IdJugador, e.IdEquipo });

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.TrayectoriaJugadorEquipo)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrayectoriaJugadorEquipo_Equipo");

            entity.HasOne(d => d.IdJugadorNavigation).WithMany(p => p.TrayectoriaJugadorEquipo)
                .HasForeignKey(d => d.IdJugador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrayectoriaJugadorEquipo_Jugador");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.Property(e => e.Apellido1)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Apellido2)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Contrasenna).IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Usuario1)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Usuario");

            entity.HasOne(d => d.RolUsuarioNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRolUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_RolUsuario");
        });

        modelBuilder.Entity<ZonaEnvio>(entity =>
        {
            entity.HasKey(e => e.IdZonaEnvio);

            entity.Property(e => e.Nombre)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
