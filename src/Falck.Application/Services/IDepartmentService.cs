using Falck.Application.DTOs;

namespace Falck.Application.Services;

/// <summary>Read-side use cases for departments.</summary>
public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
