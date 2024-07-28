using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddThreadDatabaseContextScope(this IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            if (!services.Any(s => s.ServiceType == typeof(IHostedService) && s.ImplementationType == typeof(ThreadDatabaseContextScopeHostedService)))
            {
                services.AddHostedService<ThreadDatabaseContextScopeHostedService>();
            }

            return services;
        }
    }
}