using Falck.Domain.Strategies;

namespace Falck.Tests.Domain;

public class BonusStrategyTests
{
    [Theory]
    [InlineData(1000, 100)]
    [InlineData(2500.50, 250.05)]
    [InlineData(0, 0)]
    public void RegularEmployeeStrategy_Returns10PercentOfSalary(decimal salary, decimal expected)
    {
        var strategy = new RegularEmployeeBonusStrategy();

        Assert.Equal(expected, strategy.CalculateYearlyBonus(salary));
    }

    [Theory]
    [InlineData(1000, 200)]
    [InlineData(2500.50, 500.10)]
    [InlineData(0, 0)]
    public void ManagerStrategy_Returns20PercentOfSalary(decimal salary, decimal expected)
    {
        var strategy = new ManagerBonusStrategy();

        Assert.Equal(expected, strategy.CalculateYearlyBonus(salary));
    }
}
