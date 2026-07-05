namespace Falck.Application.DTOs.Employees;

/// <summary>
/// Un período del historial de cargos de un empleado. Es un hijo del agregado
/// Employee (solo se expone dentro de <see cref="EmployeeDetailDto"/>), por eso
/// vive junto a los DTOs de empleado.
/// </summary>
public record PositionHistoryDto(string Position, DateTime StartDate, DateTime? EndDate);
