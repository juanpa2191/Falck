using Falck.Domain.Entities;

namespace Falck.Application.Interfaces;

/// <summary>Contrato de solo lectura para departamentos (usado para validar empleados).</summary>
public interface IDepartmentRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default);
}
