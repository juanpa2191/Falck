using Falck.Application.Interfaces;
using Falck.Domain.Entities;

namespace Falck.Tests.TestHelpers;

/// <summary>
/// Doble de prueba en memoria del puerto <see cref="IDepartmentRepository"/>.
/// Se configura con los departamentos que debe contener, de modo que sirve tanto
/// para validar existencia (EmployeeService) como para listar (DepartmentService).
/// Reemplaza las dos versiones anidadas que estaban duplicadas.
/// </summary>
internal sealed class FakeDepartmentRepository(params Department[] departments) : IDepartmentRepository
{
    private readonly List<Department> _departments = [.. departments];

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_departments.Any(d => d.Id == id));

    public Task<List<Department>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult(_departments.ToList());
}
