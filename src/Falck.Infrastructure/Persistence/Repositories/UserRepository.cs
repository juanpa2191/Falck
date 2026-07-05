using Falck.Application.Common.Exceptions;
using Falck.Application.Interfaces;
using Falck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Falck.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IUserRepository"/>
public class UserRepository(FalckDbContext context) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(u => u.Username == username, cancellationToken);

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Dos registros concurrentes con el mismo usuario pueden pasar ambos
            // la verificación previa de existencia; el índice único rechaza entonces al perdedor.
            throw new ConflictException($"Username '{user.Username}' is already taken.");
        }

        return user;
    }
}
