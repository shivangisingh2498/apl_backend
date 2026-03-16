using APL.Shared;
using Microsoft.AspNetCore.Http;

namespace APL.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Log the full exception (stack trace included)
                _logger.LogError(ex, "API Error");

                ErrorResponse response = new ErrorResponse
                {
                    status = "error",
                    message = ex.Message,
                    detail = ex.InnerException?.Message,
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
