using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope;

public class ThreadDatabaseContextScopeHostedService : IHostedService
{
    private readonly IProcessorThreadPoolFactory _processorThreadPoolFactory;

    public ThreadDatabaseContextScopeHostedService(IProcessorThreadPoolFactory processorThreadPoolFactory)
    {
        _processorThreadPoolFactory = Guard.AgainstNull(processorThreadPoolFactory);

        _processorThreadPoolFactory.ProcessorThreadPoolCreated += OnProcessorThreadPoolCreated;
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

    private void OnProcessorThreadPoolCreated(object? sender, ProcessorThreadPoolCreatedEventArgs e)
    {
        e.ProcessorThreadPool.ProcessorThreadCreated += ProcessorThreadCreated;
    }

    private void ProcessorThreadCreated(object? sender, ProcessorThreadCreatedEventArgs e)
    {
        e.ProcessorThread.ProcessorThreadStarting += ProcessorThreadStarting;
        e.ProcessorThread.ProcessorThreadStopping += ProcessorThreadStopping;
    }

    private void ProcessorThreadStarting(object? sender, ProcessorThreadEventArgs e)
    {
        (sender as ProcessorThread)?.State.Replace("DatabaseContextScope", new DatabaseContextScope());
    }

    private void ProcessorThreadStopping(object? sender, ProcessorThreadEventArgs e)
    {
        if (sender is not ProcessorThread processorThread)
        {
            return;
        }

        processorThread.State.Get("DatabaseContextScope")?.TryDispose();
        processorThread.State.Remove("DatabaseContextScope");

        processorThread.ProcessorThreadStarting -= ProcessorThreadStarting;
        processorThread.ProcessorThreadStopping -= ProcessorThreadStopping;
    }
}