using Falck.Domain.Enums;
using Falck.Domain.Strategies;

namespace Falck.Tests.Domain;

public class BonusStrategyFactoryTests
{
    private readonly BonusStrategyFactory _factory = new();

    [Theory]
    [InlineData(PositionType.Developer)]
    [InlineData(PositionType.Analyst)]
    [InlineData(PositionType.TeamLead)]
    public void Create_ForRegularPositions_ReturnsRegularStrategy(PositionType position)
    {
        Assert.IsType<RegularEmployeeBonusStrategy>(_factory.Create(position));
    }

    [Theory]
    [InlineData(PositionType.Manager)]
    [InlineData(PositionType.SeniorManager)]
    [InlineData(PositionType.Director)]
    public void Create_ForAnyManagerType_ReturnsManagerStrategy(PositionType position)
    {
        Assert.IsType<ManagerBonusStrategy>(_factory.Create(position));
    }
}
