using System.Diagnostics;

namespace Falck.Api.Middleware;

/// <summary>
/// Middleware personalizado (prueba técnica, sección 2.3): registra cada
/// petición HTTP entrante con método, ruta, llamante, estado de respuesta y
/// tiempo transcurrido.
///
/// El middleware en ASP.NET Core es una tubería de componentes; cada uno recibe
/// el <see cref="HttpContext"/>, puede actuar antes y después de llamar al
/// siguiente componente, y puede cortocircuitar la cadena. Este envuelve el resto
/// del pipeline en un cronómetro, así que debe registrarse temprano en Program.cs.
/// </summary>
public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            logger.LogInformation(
                "HTTP {Method} {Path}{QueryString} from {RemoteIp} as '{User}' => {StatusCode} in {ElapsedMs} ms",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                context.Connection.RemoteIpAddress,
                context.User.Identity?.IsAuthenticated == true
                    ? context.User.Identity.Name ?? "authenticated"
                    : "anonymous",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
