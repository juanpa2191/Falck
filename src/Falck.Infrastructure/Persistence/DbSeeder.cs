using Falck.Application.Interfaces;
using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Falck.Infrastructure.Persistence;

/// <summary>
/// Siembra datos en tiempo de ejecución que no pueden ir por HasData: los hashes
/// BCrypt difieren en cada ejecución, así que los usuarios de demo se crean aquí
/// (de forma idempotente) al arrancar.
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
