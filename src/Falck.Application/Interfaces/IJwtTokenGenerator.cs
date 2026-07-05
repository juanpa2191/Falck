using Falck.Domain.Entities;

namespace Falck.Application.Interfaces;

/// <summary>Emite JWT firmados para usuarios autenticados.</summary>
public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
