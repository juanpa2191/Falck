using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falck.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasData(
            new Project { Id = 1, Name = "Emergency Dispatch Platform" },
            new Project { Id = 2, Name = "Fleet Tracking System" },
            new Project { Id = 3, Name = "Internal HR Portal" });
    }
}
