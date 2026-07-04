using Falck.Domain.Entities;

namespace Falck.Application.Interfaces;

/// <summary>Issues signed JWTs for authenticated users.</summary>
public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
