using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace NL2SQL.CoreBackend.Application;

/// <summary>
/// Application katmanı servis kayıtları.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
