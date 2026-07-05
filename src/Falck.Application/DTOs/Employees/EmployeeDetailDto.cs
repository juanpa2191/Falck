using Falck.Application.DTOs.Projects;
using Falck.Domain.Enums;

namespace Falck.Application.DTOs.Employees;

/// <summary>Detalle del empleado: agrega historial de cargos y asignaciones de proyectos.</summary>
public record EmployeeDetailDto(
    int Id,
    string Name,
    PositionType CurrentPosition,
    decimal Salary,
    decimal YearlyBonus,
    int DepartmentId,
    string? DepartmentName,
    IReadOnlyList<PositionHistoryDto> PositionHistory,
    IReadOnlyList<ProjectDto> Projects);
