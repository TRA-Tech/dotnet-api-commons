using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ApiCommons.Middlewares.GlobalErrorHandler
{
    /// <summary>
    /// Provides extension methods for <see cref="IApplicationBuilder"/> to add the <see cref="GlobalErrorHandlerMiddleware"/> to the application's request pipeline.
    /// </summary>
    public static class GlobalErrorHandlerAppBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="GlobalErrorHandlerMiddleware"/> to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="exceptionHandler">The function to handle exceptions.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the app or exceptionHandler argument is null.</exception>
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app, Func<IServiceProvider, HttpContext, Exception, Task> exceptionHandler)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            return app.UseMiddleware<GlobalErrorHandlerMiddleware>(exceptionHandler);
        }
    }
}
