using Falck.Domain.Entities;

namespace Falck.Application.Interfaces;

/// <summary>
/// Repository pattern: abstracts employee persistence so the application layer
/// depends on this contract, never on EF Core (Dependency Inversion).
/// </summary>
public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches an employee with department, position history and projects.</summary>
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);

    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    /// Employees of the given department that are assigned to at least one
    /// project (technical test, section 4.3).
    /// </summary>
    Task<List<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default);
}
