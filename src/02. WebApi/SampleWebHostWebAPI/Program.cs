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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

            host = WebHost.CreateDefaultBuilder(args)
                // .ConfigureAppConfiguration(builder =>
                // {
                //     using var innerActivity = Observability.ActivitySource.StartRichActivity(logger, "ConfigureAppConfiguration.Callback", new { builder });
                   
                //     builder.Sources.Clear();
                //     builder.AddConfiguration(configuration);
                //     builder.AddUserSecrets<Program>();
                //     builder.AddEnvironmentVariables();
                // })
                // .ConfigureAppConfiguration((whbc, cb) => { return; })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    using var innerActivity = Observability.ActivitySource.StartRichActivity(logger, "ConfigureLogging.Callback", new { context, loggingBuilder });

                    var configuration = context.Configuration;

                    loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    loggingBuilder.ClearProviders();

                    var services = loggingBuilder.Services;
                    services.AddLogging(
                                 loggingBuilder =>
                                 {
                                     loggingBuilder.ClearProviders();

                                     if (configuration.GetValue("Observability:ConsoleEnabled", true))
                                     {
                                         loggingBuilder.AddDiginsightConsole();
                                     }

                                     if (configuration.GetValue("Observability:Log4NetEnabled", true))
                                     {
                                         //loggingBuilder.AddDiginsightLog4Net("log4net.config");
                                         loggingBuilder.AddDiginsightLog4Net(static sp =>
                                         {
                                             IHostEnvironment env = sp.GetRequiredService<IHostEnvironment>();
                                             string fileBaseDir = env.IsDevelopment()
                                                     ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify)
                                                     : $"{Path.DirectorySeparatorChar}home";

                                             return new IAppender[]
                                                    {
                                                            new RollingFileAppender()
                                                            {
                                                                File = Path.Combine(fileBaseDir, "LogFiles", "Diginsight", typeof(Program).Namespace!),
                                                                AppendToFile = true,
                                                                StaticLogFileName = false,
                                                                RollingStyle = RollingFileAppender.RollingMode.Composite,
                                                                DatePattern = @".yyyyMMdd.\l\o\g",
                                                                MaxSizeRollBackups = 1000,
                                                                MaximumFileSize = "100MB",
                                                                LockingModel = new FileAppender.MinimalLock(),
                                                                Layout = new DiginsightLayout()
                                                                {
                                                                    Pattern = "{Timestamp} {Category} {LogLevel} {TraceId} {Delta} {Duration} {Depth} {Indentation|-1} {Message}",
                                                                },
                                                            },
                                                    };
                                         },
                                         static _ => log4net.Core.Level.All);
                                     }
                                 }
                             );

                })
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