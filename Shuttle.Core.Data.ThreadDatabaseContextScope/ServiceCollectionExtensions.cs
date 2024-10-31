using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddThreadDatabaseContextScope(this IServiceCollection services)
    {
        Guard.AgainstNull(services);

        if (!services.Any(s => s.ServiceType == typeof(IHostedService) && s.ImplementationType == typeof(ThreadDatabaseContextScopeHostedService)))
        {
            services.AddHostedService<ThreadDatabaseContextScopeHostedService>();
        }

        return services;
    }
}