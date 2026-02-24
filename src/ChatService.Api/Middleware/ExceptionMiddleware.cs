using System.Net;
using System.Text.Json;
using ChatService.Domain.Exceptions;

namespace ChatService.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _log;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> log)
        => (_next, _log) = (next, log);

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteErrorAsync(ctx, ex);
        }
    }

    private static Task WriteErrorAsync(HttpContext ctx, Exception ex)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = ex switch
        {
            NotFoundException     => (int)HttpStatusCode.NotFound,
            UnauthorizedException => (int)HttpStatusCode.Forbidden,
            ValidationException   => (int)HttpStatusCode.UnprocessableEntity,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _                     => (int)HttpStatusCode.InternalServerError,
        };

        var body = ex is ValidationException ve
            ? JsonSerializer.Serialize(new { detail = ex.Message, errors = ve.Errors })
            : JsonSerializer.Serialize(new { detail = ex.Message });

        return ctx.Response.WriteAsync(body);
    }
}
