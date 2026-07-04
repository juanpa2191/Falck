namespace Falck.Domain.Strategies;

/// <summary>
/// Strategy pattern: each implementation encapsulates one bonus policy.
/// New policies (e.g. executives) can be added without modifying
/// <see cref="Entities.Employee"/> or existing strategies (Open/Closed).
/// </summary>
public interface IBonusStrategy
{
    /// <summary>Calculates the yearly bonus for the given salary.</summary>
    decimal CalculateYearlyBonus(decimal salary);
}
