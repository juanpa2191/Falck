using Falck.Application.Interfaces;
using Falck.Infrastructure.Authentication;
using Falck.Infrastructure.Persistence;
using Falck.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Falck.Infrastructure;

/// <summary>
/// Raíz de composición de la capa de infraestructura: la API solo llama a este
/// método y nunca referencia EF Core ni los repositorios concretos directamente.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<FalckDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                // Reintenta fallos transitorios para que la API sobreviva a SQL
                // Server aún calentando (p. ej. el contenedor de base de datos
                // arrancando junto a ella) y a cortes momentáneos de conexión.
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
