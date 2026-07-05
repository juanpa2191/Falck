namespace Falck.Domain.Enums;

/// <summary>
/// Cargos que puede ocupar un empleado. Respaldado por <see cref="int"/> para
/// que mapee directamente a la propiedad <c>CurrentPosition (int)</c> exigida
/// por el enunciado, manteniendo los valores con seguridad de tipos y
/// autodescriptivos.
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
/// Conocimiento de dominio sobre los cargos, ubicado junto al enum para que
/// agregar un nuevo tipo de gerente solo requiera tocar este archivo
/// (Open/Closed en los bordes).
/// </summary>
public static class PositionTypeExtensions
{
    /// <summary>
    /// Un cargo es gerencial cuando pertenece a la banda de gerentes (>= 10).
    /// Puede haber muchos tipos de gerente; todos comparten esta regla.
    /// </summary>
    public static bool IsManagerial(this PositionType position) =>
        (int)position >= (int)PositionType.Manager;
}
