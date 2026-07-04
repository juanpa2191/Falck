using Falck.Domain.Enums;

namespace Falck.Domain.Entities;

/// <summary>
/// One period during which an employee held a given position. The record is
/// "open" (current) while <see cref="EndDate"/> is null.
/// </summary>
public class PositionHistory
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    /// <summary>
    /// Position held during this period. Kept as string per the spec; the
    /// value is the <see cref="PositionType"/> name so it stays consistent
    /// with <see cref="Entities.Employee.CurrentPosition"/>.
    /// </summary>
    public string Position { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    /// <summary>Null while this is the employee's current position.</summary>
    public DateTime? EndDate { get; set; }

    /// <summary>Creates an open (current) history record.</summary>
    public static PositionHistory Open(int employeeId, PositionType position, DateTime startDate) =>
        new()
        {
            EmployeeId = employeeId,
            Position = position.ToString(),
            StartDate = startDate
        };

    /// <summary>Closes the record when the employee leaves the position.</summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="endDate"/> is earlier than <see cref="StartDate"/>.
    /// </exception>
    public void Close(DateTime endDate)
    {
        if (endDate < StartDate)
            throw new ArgumentException(
                $"End date {endDate:d} cannot be earlier than start date {StartDate:d}.",
                nameof(endDate));

        EndDate = endDate;
    }
}
