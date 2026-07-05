using Falck.Domain.Entities;
using Falck.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falck.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Almacenado como int, cumpliendo el requisito "CurrentPosition (int)".
        builder.Property(e => e.CurrentPosition)
            .HasConversion<int>();

        builder.Property(e => e.Salary)
            .HasPrecision(18, 2);

        // rowversion de SQL Server → concurrencia optimista en actualizaciones/eliminaciones.
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.PositionHistories)
            .WithOne(h => h.Employee)
            .HasForeignKey(h => h.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Muchos a muchos con un nombre de tabla de unión explícito y asignaciones sembradas.
        builder.HasMany(e => e.Projects)
            .WithMany(p => p.Employees)
            .UsingEntity<Dictionary<string, object>>(
                "EmployeeProjects",
                right => right.HasOne<Project>().WithMany().HasForeignKey("ProjectId"),
                left => left.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"),
                join =>
                {
                    join.HasKey("EmployeeId", "ProjectId");
                    join.HasData(
                        new { EmployeeId = 1, ProjectId = 1 },
                        new { EmployeeId = 2, ProjectId = 1 },
                        new { EmployeeId = 2, ProjectId = 2 },
                        new { EmployeeId = 4, ProjectId = 2 });
                });

        // Nota: María (5) no tiene proyecto y Ana (3) pertenece a RR. HH., así que
        // la consulta de la sección 4.3 tiene casos positivos y negativos desde el inicio.
        builder.HasData(
            new Employee { Id = 1, Name = "Laura Gómez", CurrentPosition = PositionType.Manager, Salary = 9000m, DepartmentId = 1 },
            new Employee { Id = 2, Name = "Carlos Pérez", CurrentPosition = PositionType.Developer, Salary = 5000m, DepartmentId = 1 },
            new Employee { Id = 3, Name = "Ana Rodríguez", CurrentPosition = PositionType.Analyst, Salary = 4000m, DepartmentId = 2 },
            new Employee { Id = 4, Name = "Jorge Ramírez", CurrentPosition = PositionType.Director, Salary = 12000m, DepartmentId = 3 },
            new Employee { Id = 5, Name = "María Torres", CurrentPosition = PositionType.Developer, Salary = 4800m, DepartmentId = 1 });
    }
}
