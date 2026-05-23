using System.Net;
using System.Text.Json;
using FluentValidation;

namespace TaskManagement.API.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception ex)
    {
        var (status, message) = ex switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest,
                ve.Errors.Select(e => e.ErrorMessage)),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,
                new[] { ex.Message }),
            InvalidOperationException => (HttpStatusCode.BadRequest,
                new[] { ex.Message }),
            _ => (HttpStatusCode.InternalServerError,
                new[] { "An unexpected error occurred." })
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var body = JsonSerializer.Serialize(new { errors = message });
        return context.Response.WriteAsync(body);
    }
}
