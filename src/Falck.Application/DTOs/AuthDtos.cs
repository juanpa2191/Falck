using System.ComponentModel.DataAnnotations;

namespace Falck.Application.DTOs;

/// <summary>
/// Cuerpo de la petición POST /api/auth/register. El auto-registro siempre crea
/// una cuenta User de solo lectura; deliberadamente no hay rol suministrado por
/// el cliente, así que un llamante anónimo no puede otorgarse Admin. Las cuentas
/// Admin se siembran (ver DbSeeder) o las promovería un admin existente en producción.
/// </summary>
public record RegisterRequest
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}

/// <summary>Cuerpo de la petición POST /api/auth/login.</summary>
public record LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

/// <summary>El JWT emitido más el contexto que el cliente necesita para usarlo.</summary>
public record AuthResponse(string Token, DateTime ExpiresAtUtc, string Username, string Role);
