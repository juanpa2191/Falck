using Falck.Application.DTOs.Departments;

namespace Falck.Application.Services;

/// <summary>Casos de uso de solo lectura para departamentos.</summary>
public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
