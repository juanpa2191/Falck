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
            // Self-registration is always a read-only account; never client-driven.
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
            // Run a decoy hash comparison so response time does not reveal
            // whether the username exists (mitigates user enumeration).
            passwordHasher.VerifyDecoy(request.Password);
            throw new UnauthorizedException("Invalid username or password.");
        }

        // Same generic error either way: don't leak which credential failed.
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
