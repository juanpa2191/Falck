using Falck.Domain.Enums;

namespace Falck.Domain.Strategies;

/// <inheritdoc cref="IBonusStrategyFactory"/>
public class BonusStrategyFactory : IBonusStrategyFactory
{
    public IBonusStrategy Create(PositionType position) =>
        position.IsManagerial()
            ? new ManagerBonusStrategy()
            : new RegularEmployeeBonusStrategy();
}
