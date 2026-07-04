using Falck.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Falck.Api.Middleware;

/// <summary>
/// Global error handler: converts known application exceptions into RFC 7807
/// ProblemDetails responses and shields callers from internal error details.
/// </summary>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Resource not found", ex.Message);
        }
        catch (BadRequestException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Invalid request", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError,
                "Unexpected error", "An unexpected error occurred. Please try again later.");
        }
    }

    private static Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        });
    }
}
