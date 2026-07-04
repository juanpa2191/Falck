using Falck.Domain.Enums;
using Falck.Domain.Strategies;

namespace Falck.Domain.Entities;

/// <summary>
/// An employee of the company. Tracks its current position plus the full
/// history of positions held (see <see cref="PositionHistories"/>).
/// </summary>
public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current position. Modeled as <see cref="PositionType"/> (an int-backed
    /// enum) to honor the "CurrentPosition (int)" requirement with type safety.
    /// Use <see cref="ChangePosition"/> instead of setting it directly so the
    /// position history stays consistent.
    /// </summary>
    public PositionType CurrentPosition { get; set; }

    public decimal Salary { get; set; }

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    /// <summary>History of every position this employee has held.</summary>
    public List<PositionHistory> PositionHistories { get; set; } = [];

    /// <summary>Projects the employee is currently assigned to (many-to-many).</summary>
    public List<Project> Projects { get; set; } = [];

    /// <summary>
    /// Optimistic-concurrency token (SQL Server rowversion). Lets concurrent
    /// updates/deletes of the same employee fail cleanly instead of silently
    /// producing a divergent position history or a lost update.
    /// </summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>
    /// Calculates the yearly bonus based on the salary and the current
    /// position: the factory picks the policy for the position (10% regular,
    /// 20% for any type of manager) and the strategy applies it.
    /// </summary>
    public decimal CalculateYearlyBonus(IBonusStrategyFactory strategyFactory) =>
        strategyFactory.Create(CurrentPosition).CalculateYearlyBonus(Salary);

    /// <summary>
    /// Moves the employee to a new position, closing the open history record
    /// (if any) and opening a new one starting at <paramref name="effectiveDate"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the employee already holds <paramref name="newPosition"/>.
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
