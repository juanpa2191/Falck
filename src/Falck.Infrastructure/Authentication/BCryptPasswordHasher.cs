using Falck.Application.Interfaces;

namespace Falck.Infrastructure.Authentication;

/// <summary>
/// BCrypt implementation: salted, adaptive work factor, industry standard for
/// password storage.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
