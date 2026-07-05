using AutoMapper;
using Falck.Application.DTOs;
using Falck.Domain.Entities;
using Falck.Domain.Strategies;

namespace Falck.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para la entidad de empleados y sus tipos relacionados
/// (historial de cargos y proyectos). Separado del perfil de departamentos para
/// respetar el Principio de Responsabilidad Única: cada agregado tiene su propia
/// configuración de mapeo.
/// </summary>
public class EmployeeMappingProfile : Profile
{
    // El cálculo del bono es lógica de dominio (patrones Strategy + Factory). La
    // factory no tiene estado ni dependencias, por eso se instancia una sola vez
    // aquí y se reutiliza en ambos mapeos.
    private static readonly IBonusStrategyFactory BonusFactory = new BonusStrategyFactory();

    public EmployeeMappingProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForCtorParam(nameof(EmployeeDto.YearlyBonus),
                o => o.MapFrom(src => src.CalculateYearlyBonus(BonusFactory)))
            .ForCtorParam(nameof(EmployeeDto.DepartmentName),
                o => o.MapFrom(src => src.Department != null ? src.Department.Name : null));

        CreateMap<Employee, EmployeeDetailDto>()
            .ForCtorParam(nameof(EmployeeDetailDto.YearlyBonus),
                o => o.MapFrom(src => src.CalculateYearlyBonus(BonusFactory)))
            .ForCtorParam(nameof(EmployeeDetailDto.DepartmentName),
                o => o.MapFrom(src => src.Department != null ? src.Department.Name : null))
            .ForCtorParam(nameof(EmployeeDetailDto.PositionHistory),
                o => o.MapFrom(src => src.PositionHistories.OrderBy(h => h.StartDate)));

        CreateMap<PositionHistory, PositionHistoryDto>();
        CreateMap<Project, ProjectDto>();
    }
}
