using System.ComponentModel.DataAnnotations;
using Falck.Domain.Enums;

namespace Falck.Application.DTOs.Employees;

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
