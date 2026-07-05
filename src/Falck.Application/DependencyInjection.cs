using Falck.Application.Services;
using Falck.Domain.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Falck.Application;

/// <summary>Raíz de composición de la capa de aplicación.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAuthService, AuthService>();

        // Sin estado, así que una sola instancia puede atender todas las peticiones.
        services.AddSingleton<IBonusStrategyFactory, BonusStrategyFactory>();

        return services;
    }
}
