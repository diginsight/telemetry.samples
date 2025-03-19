//using Microsoft.Owin.Hosting;

using Diginsight;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using log4net.Appender;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Web.Http.SelfHost;

namespace S10_00_AspnetWebApi480
{

    public class Program
    {
        public static IHost Host;

        //static void Main(string[] args)
        //{
        //    string baseAddress = "http://localhost:9000/";

        //    // Start the OWIN host
        //    using (WebApp.Start<Startup>(url: baseAddress))
        //    {
        //        Console.WriteLine("Web API is running on " + baseAddress);
        //        Console.WriteLine("Press Enter to exit.");
        //        Console.ReadLine();
        //    }
        //}
        public static async Task Main(string[] args)
        {
            using var observabilityManager = new ObservabilityManager();
            ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));
            Observability.LoggerFactory = observabilityManager.LoggerFactory;

            using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
            {
                var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

                Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        using var innerActivity = Observability.ActivitySource.StartRichActivity(logger, "ConfigureServices.Callback", new { context, services });

                        var configuration = context.Configuration;
                        var environment = context.HostingEnvironment;

                        services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection("Diginsight:Activities"));
                        services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>(); logger.LogDebug("services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();");
                        observabilityManager.AttachTo(services);

                        ConfigureServices(context.Configuration, services);

                        services.AddSingleton<Program>();
                    })
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
                    .UseDiginsightServiceProvider(true)
                    .Build();

                logger.LogDebug("host = appBuilder.Build(); completed");
                await Host.StartAsync(); logger.LogDebug($"await Host.StartAsync();");

                string baseAddress = "http://localhost:9000/";
                // Set up the configuration
                var config = new HttpSelfHostConfiguration(baseAddress);
                Startup startup = new Startup();
                startup.Configuration(config);

                // Create the server
                using (HttpSelfHostServer server = new HttpSelfHostServer(config))
                {
                    server.OpenAsync().Wait();
                    Console.WriteLine("Web API Self-hosted on " + baseAddress);
                    Console.WriteLine("Press Enter to quit.");
                    Console.ReadLine();
                }
            }

        }


        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            var logger = Observability.LoggerFactory.CreateLogger<Program>();
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { configuration, services });

            //services.ConfigureClassAware<AppSettingsOptions>(configuration.GetSection("AppSettings"));
            //services.ConfigureClassAware<FeatureFlagOptions>(configuration.GetSection("AppSettings"));

            //services.AddHttpClient();

            services.AddSingleton<Program>();

        }
    }
}
