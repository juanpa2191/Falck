using Falck.Domain.Enums;
using Falck.Domain.Strategies;

namespace Falck.Domain.Entities;

/// <summary>
/// Un empleado de la empresa. Registra su cargo actual más el historial
/// completo de cargos ocupados (ver <see cref="PositionHistories"/>).
/// </summary>
public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Cargo actual. Modelado como <see cref="PositionType"/> (un enum
    /// respaldado por int) para honrar el requisito "CurrentPosition (int)" con
    /// seguridad de tipos. Usa <see cref="ChangePosition"/> en vez de asignarlo
    /// directamente para que el historial de cargos se mantenga consistente.
    /// </summary>
    public PositionType CurrentPosition { get; set; }

    public decimal Salary { get; set; }

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    /// <summary>Historial de todos los cargos que ha ocupado este empleado.</summary>
    public List<PositionHistory> PositionHistories { get; set; } = [];

    /// <summary>Proyectos a los que el empleado está asignado actualmente (muchos a muchos).</summary>
    public List<Project> Projects { get; set; } = [];

    /// <summary>
    /// Token de concurrencia optimista (rowversion de SQL Server). Permite que
    /// actualizaciones/eliminaciones concurrentes del mismo empleado fallen
    /// limpiamente en vez de producir en silencio un historial de cargos
    /// divergente o una actualización perdida.
    /// </summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>
    /// Calcula el bono anual con base en el salario y el cargo actual: la
    /// factory elige la política del cargo (10% regular, 20% para cualquier
    /// tipo de gerente) y la estrategia la aplica.
    /// </summary>
    public decimal CalculateYearlyBonus(IBonusStrategyFactory strategyFactory) =>
        strategyFactory.Create(CurrentPosition).CalculateYearlyBonus(Salary);

    /// <summary>
    /// Mueve al empleado a un nuevo cargo, cerrando el registro de historial
    /// abierto (si existe) y abriendo uno nuevo a partir de
    /// <paramref name="effectiveDate"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando el empleado ya ocupa <paramref name="newPosition"/>.
    /// </exception>
    public void ChangePosition(PositionType newPosition, DateTime effectiveDate)
    {
        if (newPosition == CurrentPosition)
            throw new InvalidOperationException(
                $"Employee {Id} already holds the position {newPosition}.");

        var openRecord = PositionHistories.FirstOrDefault(h => h.EndDate is null);
        openRecord?.Close(effectiveDate);

        PositionHistories.Add(PositionHistory.Open(Id, newPosition, effectiveDate));
        CurrentPosition = newPosition;
    }
}
