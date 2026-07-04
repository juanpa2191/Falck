using Falck.Application.Services;
using Falck.Domain.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Falck.Application;

/// <summary>Composition root for the application layer.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        // Stateless, so a single instance can serve every request.
        services.AddSingleton<IBonusStrategyFactory, BonusStrategyFactory>();

        return services;
    }
}
