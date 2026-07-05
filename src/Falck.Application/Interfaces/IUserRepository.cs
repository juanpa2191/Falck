using Falck.Domain.Entities;

namespace Falck.Application.Interfaces;

/// <summary>Contrato de persistencia para las cuentas de usuario de la API.</summary>
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default);

    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
}
