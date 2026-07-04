namespace Falck.Domain.Strategies;

/// <summary>
/// Bonus policy shared by every managerial position (Manager, SeniorManager,
/// Director, ...): 20% of the salary.
/// </summary>
public class ManagerBonusStrategy : IBonusStrategy
{
    private const decimal BonusRate = 0.20m;

    public decimal CalculateYearlyBonus(decimal salary) => salary * BonusRate;
}
