using System.Net;
using System.Text.Json;
using Catalog.API.Exceptions;

namespace Catalog.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var (statusCode, message, details) = exception switch
            {
                PlateNotFoundException => (HttpStatusCode.NotFound, exception.Message, null),
                PlateValidationException validationEx => 
                    (HttpStatusCode.BadRequest, "Validation failed", validationEx.Errors),
                _ => (HttpStatusCode.InternalServerError, 
                     _environment.IsDevelopment() ? exception.Message : "An error occurred", 
                     null)
            };

            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            response.StatusCode = (int)statusCode;
            var result = JsonSerializer.Serialize(new 
            { 
                message = message,
                details = details
            });
            await response.WriteAsync(result);
        }
    }
}
