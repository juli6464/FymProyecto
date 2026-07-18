using Microsoft.EntityFrameworkCore;
using Fym.Api.Models;

namespace Fym.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Suprime explícitamente la excepción por cambios dinámicos detectados en el modelo
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Configurar Llave Primaria Compuesta para la tabla asociativa
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // 2. Configurar Relaciones y Llaves Foráneas
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3. Restricciones de unicidad (Índices)
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();

        // 4. Data Seeding (Semilla): IDs estáticos y fijos
        var adminRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userRoleId  = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var adminUserId = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "SuperAdmin", Description = "Acceso total al sistema" },
            new Role { Id = userRoleId, Name = "User", Description = "Usuario estándar" }
        );

        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = adminUserId, 
                Username = "superadmin", 
                Email = "admin@fym.com", 
                // ¡Dejamos que BCrypt calcule el hash real exacto!
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*") 
            }
        );

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = adminUserId, RoleId = adminRoleId }
        );
    }
}