using AutoMapper;
using Falck.Application.DTOs.Departments;
using Falck.Domain.Entities;

namespace Falck.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para departamentos. Aislado de
/// <see cref="EmployeeMappingProfile"/> para que la configuración de cada
/// agregado evolucione de forma independiente (Responsabilidad Única).
/// </summary>
public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        CreateMap<Department, DepartmentDto>();
    }
}
