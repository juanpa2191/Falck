using System.Diagnostics;

namespace Falck.Api.Middleware;

/// <summary>
/// Custom middleware (technical test, section 2.3): logs every incoming HTTP
/// request with method, path, caller, response status and elapsed time.
///
/// Middleware in ASP.NET Core is a pipeline of components; each one receives
/// the <see cref="HttpContext"/>, may act before and after calling the next
/// component, and may short-circuit the chain. This one wraps the rest of the
/// pipeline in a stopwatch, so it must be registered early in Program.cs.
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
