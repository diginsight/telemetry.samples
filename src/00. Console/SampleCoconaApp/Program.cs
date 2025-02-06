using Diginsight.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SampleCoconaApp;

internal class Program
{
    static void Main(string[] args)
    {
        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

        using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args })) { 
        
        
            Console.WriteLine("Hello, World!");
        }
    }
}
