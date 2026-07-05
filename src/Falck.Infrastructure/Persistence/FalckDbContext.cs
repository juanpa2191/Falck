using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Falck.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos de EF Core. Actúa además como Unit of Work: los
/// repositorios comparten la misma instancia de contexto por petición y confirman
/// los cambios mediante SaveChangesAsync.
/// </summary>
public class FalckDbContext(DbContextOptions<FalckDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<PositionHistory> PositionHistories => Set<PositionHistory>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Recoge todas las IEntityTypeConfiguration<T> de este ensamblado.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FalckDbContext).Assembly);
    }
}
