using Falck.Application.DTOs;

namespace Falck.Application.Services;

/// <summary>Casos de uso de aplicación para la entidad de empleados.</summary>
public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EmployeeDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<EmployeeDetailDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Sección 4.3: empleados de un departamento con al menos un proyecto.</summary>
    Task<List<EmployeeDto>> GetByDepartmentWithProjectsAsync(
        int departmentId, CancellationToken cancellationToken = default);
}
