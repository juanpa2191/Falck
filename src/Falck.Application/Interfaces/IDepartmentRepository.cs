using Falck.Domain.Entities;

namespace Falck.Application.Interfaces;

/// <summary>Read-side contract for departments (used to validate employees).</summary>
public interface IDepartmentRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default);
}
