using Diginsight;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using Diginsight.Stringify;
using log4net.Appender;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace SampleWpfApp;

/// <summary>Interaction logic for App.xaml</summary>
public partial class App : Application
{
    static Type T = typeof(App);
    public static IHost Host;
    public static ObservabilityManager ObservabilityManager;

    static App()
    {
        ObservabilityManager = new ObservabilityManager();
        ILogger logger = ObservabilityManager.LoggerFactory.CreateLogger(typeof(App));

        using var activity = Observability.ActivitySource.StartMethodActivity(logger);
        try
        {

        }
        catch (Exception /*ex*/) { /*sec.Exception(ex);*/ }
    }

    public App()
    {
        var logger = ObservabilityManager.LoggerFactory.CreateLogger<App>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        //var logger = Host.Services.GetRequiredService<ILogger<App>>();
        //using var activity = ActivitySource.StartMethodActivity(logger, new { });

    }

    protected async override void OnStartup(StartupEventArgs e)
    {
        var logger = ObservabilityManager.LoggerFactory.CreateLogger<App>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                using var innerActivity = Observability.ActivitySource.StartRichActivity(logger, "ConfigureServices.Callback", new { context, services });

                var configuration = context.Configuration;
                var environment = context.HostingEnvironment;

                services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection("Diginsight:Activities"));
                services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>(); logger.LogDebug("services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();");
                ObservabilityManager.AttachTo(services);

                ConfigureServices(context.Configuration, services);

                services.AddSingleton<App>();
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
                                                                File = Path.Combine(fileBaseDir, "LogFiles", "Diginsight", typeof(App).Namespace!),
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

        var mainWindow = Host.Services.GetRequiredService<MainWindow>(); logger.LogDebug($"Host.Services.GetRequiredService<MainWindow>(); returns {mainWindow.Stringify()}");

        mainWindow.Show(); logger.LogDebug($"mainWindow.Show();");
        base.OnStartup(e); logger.LogDebug($"base.OnStartup(e);");
    }


    private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        var logger = ObservabilityManager.LoggerFactory.CreateLogger<App>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { configuration, services });

        //services.ConfigureClassAware<AppSettingsOptions>(configuration.GetSection("AppSettings"));
        //services.ConfigureClassAware<FeatureFlagOptions>(configuration.GetSection("AppSettings"));

        services.AddHttpClient();

        services.AddSingleton<MainWindow>();

    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var logger = ObservabilityManager.LoggerFactory.CreateLogger<App>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { e });

        using (Host)
        {
            await Host.StopAsync(TimeSpan.FromSeconds(5));
        }

        base.OnExit(e);
    }

}
