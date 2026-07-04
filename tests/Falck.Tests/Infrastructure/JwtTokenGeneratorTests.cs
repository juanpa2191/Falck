using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Falck.Domain.Entities;
using Falck.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

namespace Falck.Tests.Infrastructure;

public class JwtTokenGeneratorTests
{
    private static readonly JwtSettings Settings = new()
    {
        Issuer = "Falck.Api",
        Audience = "Falck.Api.Clients",
        Key = "unit-test-signing-key-0123456789ABCDEF0123456789ABCDEF",
        ExpiryMinutes = 30
    };

    private readonly JwtTokenGenerator _generator = new(Options.Create(Settings));

    [Fact]
    public void GenerateToken_EmbedsRoleUsernameIssuerAndAudience()
    {
        var user = new User { Id = 42, Username = "ada", Role = Roles.Admin };

        var (token, _) = _generator.GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(Settings.Issuer, jwt.Issuer);
        Assert.Contains(Settings.Audience, jwt.Audiences);
        Assert.Equal("42", jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("ada", jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal(Roles.Admin, jwt.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
    }

    [Fact]
    public void GenerateToken_SetsExpiryFromSettings()
    {
        var before = DateTime.UtcNow;

        var (_, expiresAtUtc) = _generator.GenerateToken(new User { Id = 1, Username = "x", Role = Roles.User });

        // ExpiryMinutes = 30, allow a small execution window.
        Assert.InRange(expiresAtUtc, before.AddMinutes(29), before.AddMinutes(31));
    }

    [Fact]
    public void GenerateToken_IssuesUniqueTokenIdPerCall()
    {
        var user = new User { Id = 1, Username = "x", Role = Roles.User };
        var handler = new JwtSecurityTokenHandler();

        var first = handler.ReadJwtToken(_generator.GenerateToken(user).Token);
        var second = handler.ReadJwtToken(_generator.GenerateToken(user).Token);

        Assert.NotEqual(
            first.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value,
            second.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value);
    }
}
