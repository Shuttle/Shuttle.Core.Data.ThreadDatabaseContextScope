# Shuttle.Core.Data.ThreadDatabaseContextScope



```
PM> Install-Package Shuttle.Core.Data.ThreadDatabaseContextScope
```

Provides a mechanism to create a new database context scope per processor thread.

In order to create a new `DatabaseContextScope` per processor thread the the pools have to be created by calling the `Create` method on the `IProcessorThreadPoolFactory` implementation.

## Configuration

```c#
services.AddThreadDatabaseContextScope();
```