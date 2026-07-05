using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falck.Infrastructure.Persistence.Configurations;

public class PositionHistoryConfiguration : IEntityTypeConfiguration<PositionHistory>
{
    public void Configure(EntityTypeBuilder<PositionHistory> builder)
    {
        builder.ToTable("PositionHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Position)
            .IsRequired()
            .HasMaxLength(50);

        // El patrón de acceso común es "historial del empleado X ordenado por fecha".
        builder.HasIndex(h => new { h.EmployeeId, h.StartDate });

        builder.HasData(
            // Laura: promovida de Developer a Manager.
            new PositionHistory { Id = 1, EmployeeId = 1, Position = "Developer", StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2022, 12, 31) },
            new PositionHistory { Id = 2, EmployeeId = 1, Position = "Manager", StartDate = new DateTime(2023, 1, 1) },
            new PositionHistory { Id = 3, EmployeeId = 2, Position = "Developer", StartDate = new DateTime(2021, 3, 15) },
            new PositionHistory { Id = 4, EmployeeId = 3, Position = "Analyst", StartDate = new DateTime(2022, 6, 1) },
            // Jorge: promovido de Manager a Director.
            new PositionHistory { Id = 5, EmployeeId = 4, Position = "Manager", StartDate = new DateTime(2019, 1, 1), EndDate = new DateTime(2024, 5, 31) },
            new PositionHistory { Id = 6, EmployeeId = 4, Position = "Director", StartDate = new DateTime(2024, 6, 1) },
            new PositionHistory { Id = 7, EmployeeId = 5, Position = "Developer", StartDate = new DateTime(2023, 2, 1) });
    }
}
