using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Threading;
using System.Data.Common;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope.Tests
{
    [TestFixture]
    public class ThreadDatabaseContextScopeHostedServiceFixture
    {
        public ThreadDatabaseContextScopeHostedServiceFixture()
        {
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
        }

        [Test]
        public void Should_be_able_to_access_existing_database_context_scope()
        {
            Should_be_able_to_access_existing_database_context_scope_async(true).GetAwaiter().GetResult();
        }

        [Test]
        public async Task Should_be_able_to_access_existing_database_context_scope_async()
        {
            await Should_be_able_to_access_existing_database_context_scope_async(false);
        }

        private async Task Should_be_able_to_access_existing_database_context_scope_async(bool sync)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IProcessorThreadPoolFactory, ProcessorThreadPoolFactory>();

            services
                .AddDataAccess(builder =>
                {
                    builder.AddConnectionString("Shuttle", "Microsoft.Data.SqlClient", "Server=.;Database=Shuttle;User ID=sa;Password=Pass!000;TrustServerCertificate=true");

                    builder.Options.DatabaseContextFactory.DefaultConnectionStringName = "Shuttle";
                })
                .AddThreadDatabaseContextScope();

            var serviceProvider = services.BuildServiceProvider();

            _ = serviceProvider.GetServices<IHostedService>().OfType<ThreadDatabaseContextScopeHostedService>().Single();

            var processorThreadPoolFactory = serviceProvider.GetRequiredService<IProcessorThreadPoolFactory>();

            var processorFactory = new Mock<IProcessorFactory>();

            processorFactory.Setup(m => m.Create())
                .Returns(new Processor(serviceProvider.GetRequiredService<IDatabaseContextFactory>()));

            var processorThreadPool = processorThreadPoolFactory.Create("test", 3, processorFactory.Object, new ProcessorThreadOptions());

            if (sync)
            {
                processorThreadPool.Start();
            }
            else
            {
                await processorThreadPool.StartAsync();
            }

            await Task.Delay(2000);

            processorThreadPool.Stop();
        }
    }
}
