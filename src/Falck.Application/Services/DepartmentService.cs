using AutoMapper;
using Falck.Application.DTOs.Departments;
using Falck.Application.Interfaces;

namespace Falck.Application.Services;

/// <inheritdoc cref="IDepartmentService"/>
public class DepartmentService(IDepartmentRepository departments, IMapper mapper) : IDepartmentService
{
    public async Task<List<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await departments.GetAllAsync(cancellationToken);
        return mapper.Map<List<DepartmentDto>>(all);
    }
}
