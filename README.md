# Shuttle.Core.Data.ThreadDatabaseContextScope

This package is no longer being maintained as scopes ar eno longer supported.  Rather use the `DatabaseContextFactory` to create a new `IDatabaseContext` instance where you need it, such as in your repository implementation.

```
PM> Install-Package Shuttle.Core.Data.ThreadDatabaseContextScope
```

Provides a mechanism to create a new database context scope per processor thread.

In order to create a new `DatabaseContextScope` per processor thread the the pools have to be created by calling the `Create` method on the `IProcessorThreadPoolFactory` implementation.

## Configuration

```c#
services.AddThreadDatabaseContextScope();
```