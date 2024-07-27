using Shuttle.Core.Threading;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope.Tests;

public class Processor : IProcessor
{
    private readonly IDatabaseContextFactory _databaseContextFactory;

    public Processor(IDatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public void Execute(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (_databaseContextFactory.Create())
            {
                Console.WriteLine($"[Processor.Execute] : managed thread id = {Thread.CurrentThread.ManagedThreadId}");
            }

            Task.Delay(100, cancellationToken).GetAwaiter().GetResult();
        }
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (_databaseContextFactory.Create())
            {
                Console.WriteLine($"[Processor.ExecuteAsync] : managed thread id = {Thread.CurrentThread.ManagedThreadId}");
            }

            await Task.Delay(100, cancellationToken);
        }
    }
}