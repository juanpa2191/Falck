using AutoMapper;
using Falck.Application.DTOs;
using Falck.Domain.Entities;
using Falck.Domain.Strategies;

namespace Falck.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para el agregado Employee. Incluye el mapeo de
/// <see cref="PositionHistory"/> porque es un hijo del agregado (solo existe
/// dentro de un empleado y solo se proyecta como parte de
/// <see cref="EmployeeDetailDto"/>): comparte la misma razón para cambiar.
/// Las entidades con identidad propia (Department, Project) tienen su propio
/// perfil, respetando el Principio de Responsabilidad Única.
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

        // PositionHistory es un hijo del agregado Employee → se mapea aquí.
        // Project, entidad con identidad propia, vive en ProjectMappingProfile.
        CreateMap<PositionHistory, PositionHistoryDto>();
    }
}
