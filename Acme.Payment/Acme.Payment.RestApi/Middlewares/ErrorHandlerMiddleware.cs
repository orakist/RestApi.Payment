using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Acme.Foundation.Domain.Entities;

namespace Acme.Payment.RestApi.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.NotFound;
            var result = JsonSerializer.Serialize(new { EntityId = error.Id, Message = error.Message });

            await response.WriteAsync(result);
        }
        catch (ValidationException error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            var result = JsonSerializer.Serialize(new { Error = error.ValidationResult, Message = error.Message });

            await response.WriteAsync(result);
        }
    }
}