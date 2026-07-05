using Falck.Application.DTOs;
using Falck.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Falck.Api.Controllers;

/// <summary>Emite tokens JWT: registro e inicio de sesión (sección 3.1).</summary>
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Registra una nueva cuenta de solo lectura (User) y devuelve su primer token.</summary>
    /// <remarks>Cuentas de demo ya sembradas: admin/Admin123! y user/User123!.</remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.RegisterAsync(request, cancellationToken));

    /// <summary>Autentica a un usuario y devuelve un JWT con su claim de rol.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.LoginAsync(request, cancellationToken));
}
