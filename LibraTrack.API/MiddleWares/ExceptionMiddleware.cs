using LibraTrack.API.Errors;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LibraTrack.API.MiddleWares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Call the next middleware in the pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); // Development Logging
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError; // 500 Internal Server Error
                var response = _env.IsDevelopment()
                    ? new ApiExceptionResponse(context.Response.StatusCode, ex.Message, _env.IsDevelopment() ? ex.StackTrace?.ToString() : null)
                    : new ApiExceptionResponse(context.Response.StatusCode);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camelCase for JSON properties
                    WriteIndented = true // Format the JSON output
                };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json); // Write the response to the client
            }
        }
    }
}
