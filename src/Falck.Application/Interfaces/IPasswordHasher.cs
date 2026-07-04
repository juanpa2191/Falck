namespace Falck.Application.Interfaces;

/// <summary>
/// Hashing contract so the application layer never depends on a concrete
/// algorithm (BCrypt lives in Infrastructure).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);

    /// <summary>
    /// Verifies the password against a fixed decoy hash. Used on the
    /// unknown-username login path to keep response time constant and avoid
    /// leaking which accounts exist. The boolean result is intentionally ignored.
    /// </summary>
    bool VerifyDecoy(string password);
}
