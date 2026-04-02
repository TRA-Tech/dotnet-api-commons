using ApiCommons.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCommons.Middlewares.DbTransaction;

public class DbTransactionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<DbTransactionAttribute>();

        if (attribute is null)
        {
            await next(context);
            return;
        }

        var dbContextObj = context.RequestServices.GetService(attribute.DbContextType);
        if (dbContextObj is not DbContext dbContext)
        {
            await next(context);
            return;
        }

        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await next(context);
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
