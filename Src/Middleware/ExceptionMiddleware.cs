using System.Net;
using System.Text.Json;
using dating_course_api.Src.Errors;

namespace dating_course_api.Src.Middleware
{
    public class ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env
    )
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly IHostEnvironment _env = env;
        private readonly JsonSerializerOptions _options =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unhandled exception has occurred: {ExMessage}",
                    ex.Message
                );
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    ? new ApiException(
                        context.Response.StatusCode,
                        ex.Message,
                        ex.StackTrace?.ToString() ?? "Internal server error"
                    )
                    : new ApiException(
                        context.Response.StatusCode,
                        ex.Message,
                        "Internal server error"
                    );

                var json = JsonSerializer.Serialize(response, _options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
