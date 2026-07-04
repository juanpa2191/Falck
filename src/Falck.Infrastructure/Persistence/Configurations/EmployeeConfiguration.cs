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

        // Stored as int, satisfying the "CurrentPosition (int)" requirement.
        builder.Property(e => e.CurrentPosition)
            .HasConversion<int>();

        builder.Property(e => e.Salary)
            .HasPrecision(18, 2);

        // SQL Server rowversion → optimistic concurrency on updates/deletes.
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

        // Many-to-many with an explicit join table name and seeded assignments.
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

        // Note: María (5) has no project and Ana (3) belongs to HR, so the
        // section 4.3 query has both positive and negative cases out of the box.
        builder.HasData(
            new Employee { Id = 1, Name = "Laura Gómez", CurrentPosition = PositionType.Manager, Salary = 9000m, DepartmentId = 1 },
            new Employee { Id = 2, Name = "Carlos Pérez", CurrentPosition = PositionType.Developer, Salary = 5000m, DepartmentId = 1 },
            new Employee { Id = 3, Name = "Ana Rodríguez", CurrentPosition = PositionType.Analyst, Salary = 4000m, DepartmentId = 2 },
            new Employee { Id = 4, Name = "Jorge Ramírez", CurrentPosition = PositionType.Director, Salary = 12000m, DepartmentId = 3 },
            new Employee { Id = 5, Name = "María Torres", CurrentPosition = PositionType.Developer, Salary = 4800m, DepartmentId = 1 });
    }
}
