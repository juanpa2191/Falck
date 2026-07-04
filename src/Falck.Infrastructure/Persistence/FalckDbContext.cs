using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Falck.Infrastructure.Persistence;

/// <summary>
/// EF Core database context. Also acts as the Unit of Work: repositories share
/// the same context instance per request and commit through SaveChangesAsync.
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
        // Picks up every IEntityTypeConfiguration<T> in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FalckDbContext).Assembly);
    }
}
