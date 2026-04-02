using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCommons.Middlewares.GlobalErrorHandler;

public static class GlobalExceptionHandlerExtensions
{
    /// <summary>
    /// Registers ProblemDetails services and <typeparamref name="THandler"/> as the
    /// <see cref="IExceptionHandler"/> implementation.
    /// Call <see cref="UseGlobalErrorHandler"/> on the <see cref="IApplicationBuilder"/> to activate.
    /// </summary>
    public static IServiceCollection AddGlobalExceptionHandler<THandler>(
        this IServiceCollection services)
        where THandler : class, IExceptionHandler
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<THandler>();
        return services;
    }

    /// <summary>
    /// Adds the ASP.NET Core exception handler middleware to the pipeline.
    /// Must be called after <see cref="AddGlobalExceptionHandler{THandler}"/> during service registration.
    /// </summary>
    public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
        => app.UseExceptionHandler();
}
