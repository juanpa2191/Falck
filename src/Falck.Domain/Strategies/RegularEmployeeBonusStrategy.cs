namespace Falck.Domain.Strategies;

/// <summary>Política de bono para empleados no gerenciales: 10% del salario.</summary>
public class RegularEmployeeBonusStrategy : IBonusStrategy
{
    private const decimal BonusRate = 0.10m;

    public decimal CalculateYearlyBonus(decimal salary) => salary * BonusRate;
}
