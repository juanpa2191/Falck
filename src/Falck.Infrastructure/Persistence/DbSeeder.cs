using Falck.Application.Interfaces;
using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Falck.Infrastructure.Persistence;

/// <summary>
/// Seeds runtime data that cannot go through HasData: BCrypt hashes differ on
/// every run, so demo users are created here (idempotently) at startup.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(FalckDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync())
            return;

        context.Users.AddRange(
            new User
            {
                Username = "admin",
                PasswordHash = passwordHasher.Hash("Admin123!"),
                Role = Roles.Admin
            },
            new User
            {
                Username = "user",
                PasswordHash = passwordHasher.Hash("User123!"),
                Role = Roles.User
            });

        await context.SaveChangesAsync();
    }
}
