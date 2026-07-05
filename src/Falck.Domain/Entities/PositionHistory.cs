using Falck.Domain.Enums;

namespace Falck.Domain.Entities;

/// <summary>
/// Un período durante el cual un empleado ocupó un cargo dado. El registro está
/// "abierto" (vigente) mientras <see cref="EndDate"/> sea null.
/// </summary>
public class PositionHistory
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    /// <summary>
    /// Cargo ocupado durante este período. Se mantiene como string según el
    /// enunciado; el valor es el nombre del <see cref="PositionType"/> para que
    /// se mantenga consistente con <see cref="Entities.Employee.CurrentPosition"/>.
    /// </summary>
    public string Position { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    /// <summary>Null mientras este sea el cargo actual del empleado.</summary>
    public DateTime? EndDate { get; set; }

    /// <summary>Crea un registro de historial abierto (vigente).</summary>
    public static PositionHistory Open(int employeeId, PositionType position, DateTime startDate) =>
        new()
        {
            EmployeeId = employeeId,
            Position = position.ToString(),
            StartDate = startDate
        };

    /// <summary>Cierra el registro cuando el empleado deja el cargo.</summary>
    /// <exception cref="ArgumentException">
    /// Se lanza cuando <paramref name="endDate"/> es anterior a <see cref="StartDate"/>.
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
