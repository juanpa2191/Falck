namespace Falck.Application.Interfaces;

/// <summary>
/// Hashing contract so the application layer never depends on a concrete
/// algorithm (BCrypt lives in Infrastructure).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
