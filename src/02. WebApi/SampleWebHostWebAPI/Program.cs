using Diginsight.AspNetCore;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using log4net.Appender;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SampleWebHostWebAPI;

namespace SampleWebHostWebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        using var observabilityManager = new ObservabilityManager();
        var logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

        IWebHost host;
        using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

            host = WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.TryAddSingleton<EarlyLoggingManager>(observabilityManager);
                })
                .UseStartup<Startup>()
                .UseDiginsightServiceProvider(true)
                .Build();

            logger.LogDebug("Host built");
        }

        host.Run();
    }
}