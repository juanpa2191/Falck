using Falck.Application.Interfaces;

namespace Falck.Infrastructure.Authentication;

/// <summary>
/// BCrypt implementation: salted, adaptive work factor, industry standard for
/// password storage.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    // Computed once at type load; verifying against it costs the same as a real
    // verification, keeping the unknown-username login path constant-time.
    private static readonly string DecoyHash =
        BCrypt.Net.BCrypt.HashPassword("decoy-account-never-authenticates");

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);

    public bool VerifyDecoy(string password) =>
        BCrypt.Net.BCrypt.Verify(password, DecoyHash);
}
