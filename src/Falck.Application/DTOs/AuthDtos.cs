using System.ComponentModel.DataAnnotations;

namespace Falck.Application.DTOs;

/// <summary>
/// Payload for POST /api/auth/register. Self-registration always creates a
/// read-only User account; there is deliberately no client-supplied role, so
/// an anonymous caller cannot grant itself Admin. Admin accounts are seeded
/// (see DbSeeder) or would be promoted by an existing admin in production.
/// </summary>
public record RegisterRequest
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}

/// <summary>Payload for POST /api/auth/login.</summary>
public record LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

/// <summary>Issued JWT plus the context the client needs to use it.</summary>
public record AuthResponse(string Token, DateTime ExpiresAtUtc, string Username, string Role);
