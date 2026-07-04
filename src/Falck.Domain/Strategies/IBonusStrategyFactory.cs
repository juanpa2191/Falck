using Falck.Domain.Enums;

namespace Falck.Domain.Strategies;

/// <summary>
/// Factory pattern: resolves the <see cref="IBonusStrategy"/> that applies to
/// a given position, so callers never branch on position themselves.
/// </summary>
public interface IBonusStrategyFactory
{
    IBonusStrategy Create(PositionType position);
}
