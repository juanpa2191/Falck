namespace Falck.Application.Interfaces;

/// <summary>
/// Contrato de hashing para que la capa de aplicación nunca dependa de un
/// algoritmo concreto (BCrypt vive en Infrastructure).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);

    /// <summary>
    /// Verifica la contraseña contra un hash señuelo fijo. Se usa en el flujo de
    /// login con usuario desconocido para mantener el tiempo de respuesta
    /// constante y evitar revelar qué cuentas existen. El resultado booleano se
    /// ignora intencionalmente.
    /// </summary>
    bool VerifyDecoy(string password);
}
