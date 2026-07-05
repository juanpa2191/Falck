using Falck.Application.Interfaces;

namespace Falck.Infrastructure.Authentication;

/// <summary>
/// Implementación con BCrypt: con salt, factor de trabajo adaptativo, estándar
/// de la industria para el almacenamiento de contraseñas.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    // Se calcula una vez al cargar el tipo; verificar contra él cuesta lo mismo
    // que una verificación real, manteniendo constante el tiempo del flujo de
    // login con usuario desconocido.
    private static readonly string DecoyHash =
        BCrypt.Net.BCrypt.HashPassword("decoy-account-never-authenticates");

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);

    public bool VerifyDecoy(string password) =>
        BCrypt.Net.BCrypt.Verify(password, DecoyHash);
}
