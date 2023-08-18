using Microsoft.AspNetCore.Http;

namespace ApiCommons.Middlewares.GlobalErrorHandler
{
    /// <summary>
    /// Middleware for handling exceptions globally across the application.
    /// </summary>
    public class GlobalErrorHandlerMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _next;
        private readonly Func<IServiceProvider, HttpContext, Exception, Task> _exceptionHandler;

        public GlobalErrorHandlerMiddleware(IServiceProvider serviceProvider, RequestDelegate next, Func<IServiceProvider, HttpContext, Exception, Task> exceptionHandler)
        {
            _serviceProvider = serviceProvider;
            _next = next;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exp)
            {
                await _exceptionHandler(_serviceProvider, context, exp);
            }
        }
    }
}
