using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data.ThreadDatabaseContextScope.Tests;

public class Processor : IProcessor
{
    private readonly IDatabaseContextFactory _databaseContextFactory;

    public Processor(IDatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory);
    }

    public async Task ExecuteAsync(IProcessorThreadContext context, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using (_databaseContextFactory.Create())
            {
                Console.WriteLine($"[Processor.ExecuteAsync] : managed thread id = {Environment.CurrentManagedThreadId}");
            }

            await Task.Delay(100, cancellationToken);
        }
    }
}