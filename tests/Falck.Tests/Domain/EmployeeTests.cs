using Falck.Domain.Entities;
using Falck.Domain.Enums;
using Falck.Domain.Strategies;

namespace Falck.Tests.Domain;

public class EmployeeTests
{
    private readonly BonusStrategyFactory _strategyFactory = new();

    private static Employee CreateEmployee(PositionType position, decimal salary = 1000m) =>
        new()
        {
            Id = 1,
            Name = "Ada Lovelace",
            CurrentPosition = position,
            Salary = salary
        };

    [Fact]
    public void CalculateYearlyBonus_ForRegularEmployee_Is10Percent()
    {
        var employee = CreateEmployee(PositionType.Developer, salary: 3000m);

        Assert.Equal(300m, employee.CalculateYearlyBonus(_strategyFactory));
    }

    [Theory]
    [InlineData(PositionType.Manager)]
    [InlineData(PositionType.SeniorManager)]
    [InlineData(PositionType.Director)]
    public void CalculateYearlyBonus_ForAnyManagerType_Is20Percent(PositionType position)
    {
        var employee = CreateEmployee(position, salary: 3000m);

        Assert.Equal(600m, employee.CalculateYearlyBonus(_strategyFactory));
    }

    [Fact]
    public void ChangePosition_UpdatesCurrentPositionAndOpensHistoryRecord()
    {
        var employee = CreateEmployee(PositionType.Developer);
        var effectiveDate = new DateTime(2026, 1, 15);

        employee.ChangePosition(PositionType.Manager, effectiveDate);

        Assert.Equal(PositionType.Manager, employee.CurrentPosition);
        var record = Assert.Single(employee.PositionHistories);
        Assert.Equal(nameof(PositionType.Manager), record.Position);
        Assert.Equal(effectiveDate, record.StartDate);
        Assert.Null(record.EndDate);
    }

    [Fact]
    public void ChangePosition_ClosesPreviousHistoryRecord()
    {
        var employee = CreateEmployee(PositionType.Developer);
        employee.ChangePosition(PositionType.TeamLead, new DateTime(2025, 1, 1));

        var promotionDate = new DateTime(2026, 6, 1);
        employee.ChangePosition(PositionType.Manager, promotionDate);

        Assert.Equal(2, employee.PositionHistories.Count);
        var closed = employee.PositionHistories.Single(h => h.Position == nameof(PositionType.TeamLead));
        Assert.Equal(promotionDate, closed.EndDate);
        var open = employee.PositionHistories.Single(h => h.EndDate is null);
        Assert.Equal(nameof(PositionType.Manager), open.Position);
    }

    [Fact]
    public void ChangePosition_ToSamePosition_Throws()
    {
        var employee = CreateEmployee(PositionType.Developer);

        Assert.Throws<InvalidOperationException>(
            () => employee.ChangePosition(PositionType.Developer, DateTime.Today));
    }

    [Fact]
    public void ClosePositionHistory_WithEndDateBeforeStartDate_Throws()
    {
        var record = PositionHistory.Open(1, PositionType.Developer, new DateTime(2026, 5, 1));

        Assert.Throws<ArgumentException>(() => record.Close(new DateTime(2026, 4, 1)));
    }
}
