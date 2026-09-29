using System.Net;
using System.Text.Json;

namespace server.Middleware
{
    // ============================================================================
    // EXCEPTION MIDDLEWARE - Xử lý exception toàn cục
    // ============================================================================
    // Tại sao cần: Thay thể try-catch ở mỗi controller
    // ============================================================================
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = exception.Message,
                // Chỉ hiển thị stack trace trong Development
                stackTrace = context.RequestServices.GetService<IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? exception.StackTrace
                    : null
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    // Extension method để dễ sử dụng
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
