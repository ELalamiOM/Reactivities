using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;


namespace Reactivity.Api.Middleware;

public class ExceptionMiddleware(IHostEnvironment env, ILogger<ExceptionMiddleware> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized access");
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized,
                "Unauthorized", ex.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception occurred");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                env.IsDevelopment() ? exception.Message : "An internal server error has occurred");
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, ValidationException ex)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if(ex.Errors is not null)
        {
            foreach (var error in ex.Errors)
            {
                validationErrors[error.PropertyName] = new[] { error.ErrorMessage };
            }
        }
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var validationProblemDetails = new ValidationProblemDetails(validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type ="Validation Failure",
            Title = "Validation Error",
            Detail = "One or more validation errors occurred."
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(validationProblemDetails,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }

}
