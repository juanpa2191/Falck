using Falck.Application.DTOs;

namespace Falck.Application.Services;

/// <summary>Authentication use cases: registration and login (section 3.1).</summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
