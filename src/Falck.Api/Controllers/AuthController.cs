using Falck.Application.DTOs;
using Falck.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Falck.Api.Controllers;

/// <summary>Issues JWT tokens: registration and login (section 3.1).</summary>
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Registers a new read-only (User) account and returns its first token.</summary>
    /// <remarks>Demo accounts already seeded: admin/Admin123! and user/User123!.</remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.RegisterAsync(request, cancellationToken));

    /// <summary>Authenticates a user and returns a JWT with its role claim.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.LoginAsync(request, cancellationToken));
}
