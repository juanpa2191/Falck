using AutoMapper;
using Falck.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Falck.Tests.TestHelpers;

/// <summary>
/// Construye un <see cref="IMapper"/> real a partir de los perfiles de la capa
/// de aplicación, por el mismo camino que producción (AddAutoMapper). Así los
/// tests ejercitan la configuración de mapeo verdadera, no un doble.
/// </summary>
internal static class TestMapper
{
    public static IMapper Create()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(typeof(EmployeeMappingProfile).Assembly);
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
}
