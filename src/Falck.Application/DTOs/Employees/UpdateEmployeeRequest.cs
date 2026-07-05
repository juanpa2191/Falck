using System.ComponentModel.DataAnnotations;
using Falck.Domain.Enums;

namespace Falck.Application.DTOs.Employees;

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
