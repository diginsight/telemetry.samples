//using Microsoft.Owin.Hosting;

using Diginsight;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using log4net.Appender;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Web.Http.SelfHost;

namespace S10_00_AspnetWebApi480SdkProject
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
            LoggerFactoryStaticAccessor.LoggerFactory = observabilityManager.LoggerFactory;
            //ObservabilityRegistry.RegisterLoggerFactory(observabilityManager.LoggerFactory);
            ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

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
                        services.TryAddSingleton<IActivityLoggingFilter, OptionsBasedActivityLoggingFilter>(); logger.LogDebug("services.TryAddSingleton<IActivityLoggingFilter, OptionsBasedActivityLoggingFilter>();");

                        // Add logging and opentelemetry
                        services.AddObservability(configuration, environment); logger.LogDebug("services.AddObservability(configuration, environment);");

                        observabilityManager.AttachTo(services);

                        ConfigureServices(context.Configuration, services); logger.LogDebug("ConfigureServices(context.Configuration, services);");

                        services.AddSingleton<Program>();
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
            var logger = LoggerFactoryStaticAccessor.LoggerFactory?.CreateLogger<Program>();
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { configuration, services });

            //services.ConfigureClassAware<AppSettingsOptions>(configuration.GetSection("AppSettings"));
            //services.ConfigureClassAware<FeatureFlagOptions>(configuration.GetSection("AppSettings"));

            //services.AddHttpClient();

            services.AddSingleton<Program>();

        }
    }
}
