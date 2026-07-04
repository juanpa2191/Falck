using Falck.Application.Common.Exceptions;
using Falck.Application.DTOs;
using Falck.Application.Interfaces;
using Falck.Application.Services;
using Falck.Domain.Entities;

namespace Falck.Tests.Application;

public class AuthServiceTests
{
    private readonly FakeUserRepository _users = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_users, new FakePasswordHasher(), new FakeTokenGenerator());
    }

    [Fact]
    public async Task Register_NewUsername_StoresHashedPasswordAndReturnsToken()
    {
        var response = await _service.RegisterAsync(new RegisterRequest
        {
            Username = "ada",
            Password = "Sup3rSecret!",
            Role = Roles.Admin
        });

        Assert.Equal("token-for-ada", response.Token);
        Assert.Equal(Roles.Admin, response.Role);
        var stored = Assert.Single(_users.Users);
        Assert.Equal("hashed:Sup3rSecret!", stored.PasswordHash);
    }

    [Fact]
    public async Task Register_DuplicateUsername_ThrowsBadRequest()
    {
        _users.Users.Add(new User { Username = "ada", PasswordHash = "x" });

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _service.RegisterAsync(new RegisterRequest { Username = "ada", Password = "Sup3rSecret!" }));
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokenWithRole()
    {
        _users.Users.Add(new User
        {
            Username = "ada",
            PasswordHash = "hashed:Sup3rSecret!",
            Role = Roles.User
        });

        var response = await _service.LoginAsync(new LoginRequest
        {
            Username = "ada",
            Password = "Sup3rSecret!"
        });

        Assert.Equal("token-for-ada", response.Token);
        Assert.Equal(Roles.User, response.Role);
    }

    [Theory]
    [InlineData("ada", "WrongPassword")]
    [InlineData("nobody", "Sup3rSecret!")]
    public async Task Login_InvalidCredentials_ThrowsUnauthorized(string username, string password)
    {
        _users.Users.Add(new User { Username = "ada", PasswordHash = "hashed:Sup3rSecret!" });

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _service.LoginAsync(new LoginRequest { Username = username, Password = password }));
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
            Task.FromResult(Users.FirstOrDefault(u => u.Username == username));

        public Task<bool> ExistsAsync(string username, CancellationToken ct = default) =>
            Task.FromResult(Users.Any(u => u.Username == username));

        public Task<User> AddAsync(User user, CancellationToken ct = default)
        {
            Users.Add(user);
            return Task.FromResult(user);
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hashed:{password}";

        public bool Verify(string password, string passwordHash) =>
            passwordHash == $"hashed:{password}";
    }

    private sealed class FakeTokenGenerator : IJwtTokenGenerator
    {
        public (string Token, DateTime ExpiresAtUtc) GenerateToken(User user) =>
            ($"token-for-{user.Username}", DateTime.UtcNow.AddHours(1));
    }
}
