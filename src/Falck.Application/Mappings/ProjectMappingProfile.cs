using AutoMapper;
using Falck.Application.DTOs.Projects;
using Falck.Domain.Entities;

namespace Falck.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para proyectos. Aunque hoy solo se proyecta anidado en
/// el detalle del empleado, <see cref="Project"/> es una entidad propia (tabla
/// propia, relación N:N) con su propia razón para cambiar, por eso tiene su
/// perfil independiente —igual que los departamentos.
/// </summary>
public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>();
    }
}
