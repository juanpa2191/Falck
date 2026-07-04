using Falck.Application.DTOs;

namespace Falck.Application.Services;

/// <summary>Application use cases for the employees entity.</summary>
public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EmployeeDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<EmployeeDetailDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Section 4.3: employees of a department with at least one project.</summary>
    Task<List<EmployeeDto>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default);
}
