namespace Falck.Domain.Strategies;

/// <summary>Bonus policy for non-managerial employees: 10% of the salary.</summary>
public class RegularEmployeeBonusStrategy : IBonusStrategy
{
    private const decimal BonusRate = 0.10m;

    public decimal CalculateYearlyBonus(decimal salary) => salary * BonusRate;
}
