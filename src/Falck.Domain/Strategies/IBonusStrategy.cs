namespace Falck.Domain.Strategies;

/// <summary>
/// Patrón Strategy: cada implementación encapsula una política de bono.
/// Se pueden agregar nuevas políticas (p. ej. ejecutivos) sin modificar
/// <see cref="Entities.Employee"/> ni las estrategias existentes (Open/Closed).
/// </summary>
public interface IBonusStrategy
{
    /// <summary>Calcula el bono anual para el salario dado.</summary>
    decimal CalculateYearlyBonus(decimal salary);
}
