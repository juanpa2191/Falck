namespace Falck.Domain.Strategies;

/// <summary>
/// Política de bono compartida por todos los cargos gerenciales (Manager,
/// SeniorManager, Director, ...): 20% del salario.
/// </summary>
public class ManagerBonusStrategy : IBonusStrategy
{
    private const decimal BonusRate = 0.20m;

    public decimal CalculateYearlyBonus(decimal salary) => salary * BonusRate;
}
