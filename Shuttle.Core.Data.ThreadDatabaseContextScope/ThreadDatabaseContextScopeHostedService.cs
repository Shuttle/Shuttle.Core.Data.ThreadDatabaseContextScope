using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope
{
    public class ThreadDatabaseContextScopeHostedService : IHostedService
    {
        private readonly IProcessorThreadPoolFactory _processorThreadPoolFactory;

        public ThreadDatabaseContextScopeHostedService(IProcessorThreadPoolFactory processorThreadPoolFactory)
        {
            _processorThreadPoolFactory = Guard.AgainstNull(processorThreadPoolFactory, nameof(processorThreadPoolFactory));

            _processorThreadPoolFactory.ProcessorThreadPoolCreated += OnProcessorThreadPoolCreated;
        }

        private void OnProcessorThreadPoolCreated(object sender, ProcessorThreadPoolCreatedEventArgs e)
        {
            e.ProcessorThreadPool.ProcessorThreadCreated += ProcessorThreadCreated;
        }

        private void ProcessorThreadCreated(object sender, ProcessorThreadCreatedEventArgs e)
        {
            e.ProcessorThread.ProcessorThreadStarting += ProcessorThreadStarting;
            e.ProcessorThread.ProcessorThreadStopped += ProcessorThreadStopped;
        }

        private void ProcessorThreadStopped(object sender, ProcessorThreadStoppedEventArgs e)
        {
            var processorThread = (sender as ProcessorThread);

            if (processorThread == null)
            {
                return;
            }

            processorThread.GetState("DatabaseContextScope")?.TryDispose();

            processorThread.ProcessorThreadStarting -= ProcessorThreadStarting;
            processorThread.ProcessorThreadStopped -= ProcessorThreadStopped;
        }

        private void ProcessorThreadStarting(object sender, ProcessorThreadEventArgs e)
        {
            (sender as ProcessorThread)?.SetState("DatabaseContextScope", new DatabaseContextScope());
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _processorThreadPoolFactory.ProcessorThreadPoolCreated -= OnProcessorThreadPoolCreated;

            await Task.CompletedTask;
        }
    }
}
