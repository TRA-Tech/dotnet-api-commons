using ApiCommons.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCommons.Middlewares.DbTransaction
{
    /// <summary>
    /// Middleware for managing database transactions.
    /// </summary>
    public class DbTransactionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, HttpContext, Exception, Task>? _exceptionHandler;

        public DbTransactionMiddleware(IServiceProvider serviceProvider, RequestDelegate next)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public DbTransactionMiddleware(IServiceProvider serviceProvider, RequestDelegate next, Func<IServiceProvider, HttpContext, Exception, Task> exceptionHandler)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            DbTransactionAttribute? attribute = endpoint?.Metadata.GetMetadata<DbTransactionAttribute>();
            if (attribute == null)
            {
                await _next(context);
                return;
            }

            await using var asyncServiceScope = _serviceProvider.CreateAsyncScope();
            DbContext dbContext = (DbContext)asyncServiceScope.ServiceProvider.GetRequiredService(attribute.DbContextType);
            if (dbContext == null)
            {
                await _next(context);
                return;
            }

            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await _next(context);
                await transaction.CommitAsync();
            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                if (_exceptionHandler is not null) await _exceptionHandler(_serviceProvider, context, exp);
                else throw;
            }
        }
    }
}
