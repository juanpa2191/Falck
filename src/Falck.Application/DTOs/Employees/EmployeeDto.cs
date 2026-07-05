using Falck.Domain.Enums;

namespace Falck.Application.DTOs.Employees;

/// <summary>Empleado tal como lo exponen los endpoints de listado. Las entidades nunca salen de la API.</summary>
public record EmployeeDto(
    int Id,
    string Name,
    PositionType CurrentPosition,
    decimal Salary,
    decimal YearlyBonus,
    int DepartmentId,
    string? DepartmentName);
