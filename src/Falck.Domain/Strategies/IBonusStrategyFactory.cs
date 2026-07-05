using Falck.Domain.Enums;

namespace Falck.Domain.Strategies;

/// <summary>
/// Patrón Factory: resuelve la <see cref="IBonusStrategy"/> que aplica a un
/// cargo dado, de modo que quien la invoca nunca ramifica según el cargo.
/// </summary>
public interface IBonusStrategyFactory
{
    IBonusStrategy Create(PositionType position);
}
