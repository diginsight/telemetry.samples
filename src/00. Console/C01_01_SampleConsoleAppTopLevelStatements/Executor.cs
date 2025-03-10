using Diginsight.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SampleConsoleAppTopLevelStatementsTopLevelStatements;

public class Executor
{
    private readonly ILogger<Executor> logger;
    
    public Executor(ILogger<Executor> logger)
    {
        this.logger = logger;
    }

    public Task ExecuteAsync()
    {
        using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger);

        // Console.WriteLine("Executing one-time method...");
        // Add your one-time method logic here

        return Task.CompletedTask;
    }
}
