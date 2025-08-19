using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ApiCommons.Middlewares.DbTransaction
{
    /// <summary>
    /// Provides extension methods for <see cref="IApplicationBuilder"/> to add the <see cref="DbTransactionMiddleware"/> to the application's request pipeline.
    /// </summary>
    public static class DbTransactionAppBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="DbTransactionMiddleware"/> to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="exceptionHandler">An optional function to handle exceptions.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the app argument is null.</exception>
        public static IApplicationBuilder UseDbTransaction(this IApplicationBuilder app, Func<IServiceProvider, HttpContext, Exception, Task>? exceptionHandler = null)
        {
            if (exceptionHandler is not null)
                return app.UseMiddleware<DbTransactionMiddleware>(exceptionHandler);

            return app.UseMiddleware<DbTransactionMiddleware>();
        }
    }
}
