using Falck.Application.Services;
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

        // Registra IMapper y descubre todos los Profile de este ensamblado
        // (EmployeeMappingProfile, DepartmentMappingProfile).
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}
