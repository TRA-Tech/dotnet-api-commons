using Microsoft.AspNetCore.Builder;

namespace ApiCommons.Middlewares.DbTransaction;

public static class DbTransactionAppBuilderExtensions
{
    public static IApplicationBuilder UseDbTransaction(this IApplicationBuilder app)
        => app.UseMiddleware<DbTransactionMiddleware>();
}
