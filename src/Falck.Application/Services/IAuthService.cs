using Falck.Application.DTOs.Auth;

namespace Falck.Application.Services;

/// <summary>Casos de uso de autenticación: registro e inicio de sesión (sección 3.1).</summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
