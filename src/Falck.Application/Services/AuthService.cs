using Falck.Application.Common.Exceptions;
using Falck.Application.DTOs;
using Falck.Application.Interfaces;
using Falck.Domain.Entities;

namespace Falck.Application.Services;

/// <inheritdoc cref="IAuthService"/>
public class AuthService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await users.ExistsAsync(request.Username, cancellationToken))
            throw new BadRequestException($"Username '{request.Username}' is already taken.");

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHasher.Hash(request.Password),
            // El auto-registro siempre es una cuenta de solo lectura; nunca lo define el cliente.
            Role = Roles.User
        };

        await users.AddAsync(user, cancellationToken);

        return BuildResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null)
        {
            // Ejecuta una comparación de hash señuelo para que el tiempo de
            // respuesta no revele si el usuario existe (mitiga la enumeración de usuarios).
            passwordHasher.VerifyDecoy(request.Password);
            throw new UnauthorizedException("Invalid username or password.");
        }

        // Mismo error genérico en ambos casos: no revelar qué credencial falló.
        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        return BuildResponse(user);
    }

    private AuthResponse BuildResponse(User user)
    {
        var (token, expiresAtUtc) = tokenGenerator.GenerateToken(user);
        return new AuthResponse(token, expiresAtUtc, user.Username, user.Role);
    }
}
