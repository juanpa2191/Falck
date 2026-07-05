using System.ComponentModel.DataAnnotations;

namespace Falck.Application.DTOs.Auth;

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
