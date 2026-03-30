using ProjectManagement.Api.Errors;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Domain.Exceptions;
using Serilog;
using System.Text.Json;
using static ProjectManagement.Api.Common.Constants.ApiConstants;

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
                StatusCodes.Status400BadRequest,
                ErrorTypes.Validation,
                ErrorMessageTranslator.Translate(ex.Message),
                ex.Errors.Select(e => new
                {
                    e.Property,
                    Message = ErrorMessageTranslator.Translate(e.Message)
                }).Cast<object>().ToArray()),

            NotFoundException ex => new ApiErrorResponse(
                StatusCodes.Status404NotFound,
                ErrorTypes.NotFound,
                ErrorMessageTranslator.Translate(ex.Message),
                [new { Entity = ex.EntityName, ex.Key }]),

            DomainException ex => new ApiErrorResponse(
                StatusCodes.Status422UnprocessableEntity,
                ErrorTypes.Domain,
                ErrorMessageTranslator.Translate(ex.Error.MessageKey),
                ex.PropertyName is null
                    ? Array.Empty<object>()
                    : [new { Property = ex.PropertyName }]),

            _ => new ApiErrorResponse(
                StatusCodes.Status500InternalServerError,
                ErrorTypes.InternalServer,
                Messages.InternalServerError,
                Array.Empty<object>())
        };

        if (response.Status >= 500)
        {
            Log.Error(exception, LogTemplates.CriticalError, context.Request.Method, context.Request.Path);
        }
        else
        {
            Log.Warning(exception, LogTemplates.ControlledError,
                response.Status, context.Request.Method, context.Request.Path, response.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.Status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}