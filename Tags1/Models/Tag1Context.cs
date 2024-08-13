using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Tags1.Models;

public partial class Tag1Context : DbContext
{
    public Tag1Context()
    {
    }

    public Tag1Context(DbContextOptions<Tag1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Activo> Activos { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Tipoactivo> Tipoactivos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=CadenaSQL");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("activo_pkey");

            entity.ToTable("activo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Supervisor).HasColumnName("supervisor");
            entity.Property(e => e.TipoActivo).HasColumnName("tipo_activo");

            entity.HasOne(d => d.AreaNavigation).WithMany(p => p.Activos)
                .HasForeignKey(d => d.Area)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("activo_area_fkey");

            entity.HasOne(d => d.SupervisorNavigation).WithMany(p => p.Activos)
                .HasForeignKey(d => d.Supervisor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("activo_supervisor_fkey");

            entity.HasOne(d => d.TipoActivoNavigation).WithMany(p => p.Activos)
                .HasForeignKey(d => d.TipoActivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("activo_tipo_activo_fkey");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("area_pkey");

            entity.ToTable("area");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Area1)
                .HasMaxLength(50)
                .HasColumnName("area");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rol_pkey");

            entity.ToTable("rol");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rol1)
                .HasMaxLength(50)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<Tipoactivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipoactivo_pkey");

            entity.ToTable("tipoactivo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Correo, "usuario_correo_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(100)
                .HasColumnName("contraseña");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Rol).HasColumnName("rol");

            entity.HasOne(d => d.RolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Rol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_rol_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
