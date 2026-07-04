using System.ComponentModel.DataAnnotations;
using Falck.Domain.Enums;

namespace Falck.Application.DTOs;

/// <summary>Employee as exposed by list endpoints. Entities never leave the API.</summary>
public record EmployeeDto(
    int Id,
    string Name,
    PositionType CurrentPosition,
    decimal Salary,
    decimal YearlyBonus,
    int DepartmentId,
    string? DepartmentName);

/// <summary>Employee detail: adds position history and project assignments.</summary>
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

/// <summary>Payload for POST /api/employees.</summary>
public record CreateEmployeeRequest
{
    [Required, StringLength(100)]
    public string Name { get; init; } = string.Empty;

    /// <summary>Accepts the enum name ("Manager") or its numeric value (10).</summary>
    [EnumDataType(typeof(PositionType))]
    public PositionType CurrentPosition { get; init; }

    [Range(0.01, 100_000_000)]
    public decimal Salary { get; init; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; init; }
}

/// <summary>
/// Payload for PUT /api/employees/{id}. Changing <see cref="CurrentPosition"/>
/// automatically records the change in the employee's position history.
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
