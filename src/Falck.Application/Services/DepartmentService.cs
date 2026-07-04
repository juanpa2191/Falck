using Falck.Application.DTOs;
using Falck.Application.Interfaces;
using Falck.Application.Mappings;

namespace Falck.Application.Services;

/// <inheritdoc cref="IDepartmentService"/>
public class DepartmentService(IDepartmentRepository departments) : IDepartmentService
{
    public async Task<List<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await departments.GetAllAsync(cancellationToken);
        return all.Select(d => d.ToDto()).ToList();
    }
}
