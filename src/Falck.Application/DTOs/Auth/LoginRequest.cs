using System.ComponentModel.DataAnnotations;

namespace Falck.Application.DTOs.Auth;

/// <summary>Cuerpo de la petición POST /api/auth/login.</summary>
public record LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
