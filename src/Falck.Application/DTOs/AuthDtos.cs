using System.ComponentModel.DataAnnotations;
using Falck.Domain.Entities;

namespace Falck.Application.DTOs;

/// <summary>Payload for POST /api/auth/register.</summary>
public record RegisterRequest
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// "Admin" or "User". Defaults to "User". Exposed only so the technical
    /// test can be exercised end to end; in production role assignment would
    /// be an admin-only operation.
    /// </summary>
    [RegularExpression($"^({Roles.Admin}|{Roles.User})$",
        ErrorMessage = "Role must be 'Admin' or 'User'.")]
    public string Role { get; init; } = Roles.User;
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
