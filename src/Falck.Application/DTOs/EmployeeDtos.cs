using System.ComponentModel.DataAnnotations;
using Falck.Domain.Enums;

namespace Falck.Application.DTOs;

/// <summary>Empleado tal como lo exponen los endpoints de listado. Las entidades nunca salen de la API.</summary>
public record EmployeeDto(
    int Id,
    string Name,
    PositionType CurrentPosition,
    decimal Salary,
    decimal YearlyBonus,
    int DepartmentId,
    string? DepartmentName);

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

public record PositionHistoryDto(string Position, DateTime StartDate, DateTime? EndDate);

public record ProjectDto(int Id, string Name);

public record DepartmentDto(int Id, string Name);

/// <summary>Cuerpo de la petición POST /api/employees.</summary>
public record CreateEmployeeRequest
{
    [Required, StringLength(100)]
    public string Name { get; init; } = string.Empty;

    /// <summary>Acepta el nombre del enum ("Manager") o su valor numérico (10).</summary>
    [EnumDataType(typeof(PositionType))]
    public PositionType CurrentPosition { get; init; }

    [Range(0.01, 100_000_000)]
    public decimal Salary { get; init; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; init; }
}

/// <summary>
/// Cuerpo de la petición PUT /api/employees/{id}. Se mantiene como un record
/// independiente (paralelo a <see cref="CreateEmployeeRequest"/>) para que los
/// dos cuerpos de petición puedan evolucionar por separado. Cambiar
/// <see cref="CurrentPosition"/> registra automáticamente el cambio en el
/// historial de cargos del empleado.
/// </summary>
public record UpdateEmployeeRequest
{
    [Required, StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [EnumDataType(typeof(PositionType))]
    public PositionType CurrentPosition { get; init; }

    [Range(0.01, 100_000_000)]
    public decimal Salary { get; init; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; init; }
}
