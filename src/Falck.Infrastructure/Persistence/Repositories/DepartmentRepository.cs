using Falck.Application.Interfaces;
using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Falck.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IDepartmentRepository"/>
public class DepartmentRepository(FalckDbContext context) : IDepartmentRepository
{
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.Departments.AnyAsync(d => d.Id == id, cancellationToken);

    public Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default) =>
        context.Departments.AsNoTracking().OrderBy(d => d.Id).ToListAsync(cancellationToken);
}
