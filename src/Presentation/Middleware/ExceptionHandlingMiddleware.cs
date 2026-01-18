using System.Net;
using System.Text.Json;
using TaskRtUpdater.src.Domain.Exceptions;

namespace TaskRtUpdater.src.Presentation.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception happened.");
                await HandleGeneralExceptionAsync(context, ex);
            }
        }

        private static Task HandleGeneralExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new { error = "", statusCode = HttpStatusCode.InternalServerError };

            switch (exception)
            {
                case DependencyNotFoundException depEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new { error = depEx.Message, statusCode = HttpStatusCode.BadRequest };
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new { error = exception.Message, statusCode = HttpStatusCode.BadRequest };
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new { error = "Internal server error.", statusCode = HttpStatusCode.InternalServerError };
                    break;
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}