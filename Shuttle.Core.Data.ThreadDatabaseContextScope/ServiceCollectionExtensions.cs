using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddThreadDatabaseContextScope(this IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            services.AddHostedService<ThreadDatabaseContextScopeHostedService>();

            return services;
        }
    }
}