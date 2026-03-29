using System.Text.Json;
using ProjectManagement.Api.Errors;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Domain.Exceptions;
using Serilog;

namespace ProjectManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = exception switch
        {
            RequestValidationException ex => new ApiErrorResponse(
                400,
                "ValidationException",
                ErrorMessageTranslator.Translate(ex.Message),
                ex.Errors.Select(e => new
                {
                    e.Property,
                    Message = ErrorMessageTranslator.Translate(e.Message)
                }).Cast<object>().ToArray()),

            NotFoundException ex => new ApiErrorResponse(
                404,
                "NotFoundException",
                ErrorMessageTranslator.Translate(ex.Message),
                [new { Entity = ex.EntityName, ex.Key }]),

            DomainException ex => new ApiErrorResponse(
                422,
                "DomainException",
                ErrorMessageTranslator.Translate(ex.Error.MessageKey),
                ex.PropertyName is null
                    ? Array.Empty<object>()
                    : [new { Property = ex.PropertyName }]),

            _ => new ApiErrorResponse(
                500,
                "InternalServerError",
                "Error interno no controlado.",
                Array.Empty<object>())
        };

        if (response.Status >= 500)
        {
            Log.Error(
                exception,
                "Error crítico en API. {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }
        else
        {
            Log.Warning(
                exception,
                "Error controlado en API. {Status} {Method} {Path} - {Message}",
                response.Status,
                context.Request.Method,
                context.Request.Path,
                response.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.Status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}