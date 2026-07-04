namespace Falck.Domain.Enums;

/// <summary>
/// Positions an employee can hold. Backed by <see cref="int"/> so it maps
/// directly to the <c>CurrentPosition (int)</c> property required by the spec,
/// while keeping the values type-safe and self-describing.
/// </summary>
public enum PositionType
{
    Developer = 1,
    Analyst = 2,
    TeamLead = 3,
    Manager = 10,
    SeniorManager = 11,
    Director = 12
}

/// <summary>
/// Domain knowledge about positions, kept next to the enum so adding a new
/// manager type only requires touching this file (Open/Closed at the edges).
/// </summary>
public static class PositionTypeExtensions
{
    /// <summary>
    /// A position is managerial when it belongs to the manager band (>= 10).
    /// There can be many types of managers; all of them share this rule.
    /// </summary>
    public static bool IsManagerial(this PositionType position) =>
        (int)position >= (int)PositionType.Manager;
}
