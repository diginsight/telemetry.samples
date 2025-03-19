using Diginsight;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using Diginsight.Options;
using log4net.Appender;
using log4net.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace S10_00_AspnetWebApi480;

using Options = Microsoft.Extensions.Options.Options;

public static partial class ObservabilityExtensions
{
    static Type T = typeof(ObservabilityExtensions);

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        bool configureDefaults = true
    )
    {
        var loggerFactory = Observability.LoggerFactory;
        var logger = loggerFactory.CreateLogger(T);
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { services, configuration, hostEnvironment, configureDefaults });

        bool isLocal = hostEnvironment.IsDevelopment(); logger.LogDebug("isLocal: {isLocal}", isLocal);
        string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!; logger.LogDebug("assemblyName: {assemblyName}", assemblyName);

        IConfiguration openTelemetryConfiguration = configuration.GetSection("OpenTelemetry"); logger.LogDebug("openTelemetryConfiguration: {openTelemetryConfiguration}", openTelemetryConfiguration);

        services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>(); logger.LogDebug("services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();");

        //services.TryAddEnumerable(ServiceDescriptor.Singleton<IActivityListenerRegistration, ActivitySourceDetectorRegistration>()); logger.LogDebug("services.TryAddEnumerable(ServiceDescriptor.Singleton<IActivityListenerRegistration, ActivitySourceDetectorRegistration>());");

        services.AddLogging(
            loggingBuilder =>
            {
                using var activityInner = Observability.ActivitySource.StartRichActivity(logger, "AddLogging.Configure", () => new { loggingBuilder });
                
                loggingBuilder.ClearProviders(); logger.LogDebug("loggingBuilder.ClearProviders();");

                var consoleEnabled = configuration.GetValue(ConfigurationPath.Combine("Observability", "ConsoleEnabled"), true); logger.LogDebug("consoleEnabled: {consoleEnabled}", consoleEnabled);
                if (consoleEnabled)
                {
                    loggingBuilder.AddDiginsightConsole(
                        fo =>
                        {
                            if (configureDefaults)
                            {
                                fo.TotalWidth = isLocal ? -1 : 0;
                                fo.UseColor = isLocal;
                            }
                            configuration.GetSection(ConfigurationPath.Combine("Diginsight", "Console")).Bind(fo);
                        }
                    );
                    logger.LogDebug("loggingBuilder.AddDiginsightConsole();");
                }

                var log4NetEnabled = configuration.GetValue(ConfigurationPath.Combine("Observability", "Log4NetEnabled"), true); logger.LogDebug("log4NetEnabled: {log4NetEnabled}", log4NetEnabled);
                if (log4NetEnabled)
                {
                    loggingBuilder.AddDiginsightLog4Net(
                        sp =>
                        {
                            IHostEnvironment env = sp.GetRequiredService<IHostEnvironment>();
                            string fileBaseDir = env.IsDevelopment()
                                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify)
                                : $"{Path.DirectorySeparatorChar}home";
                            logger.LogDebug("fileBaseDir: {fileBaseDir}", fileBaseDir);

                            return
                            [
                                new RollingFileAppender()
                                {
                                    File = Path.Combine(fileBaseDir, "LogFiles", "Diginsight", assemblyName),
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
                            ];
                        },
                        static _ => Level.All
                    );
                    logger.LogDebug("loggingBuilder.AddDiginsightLog4Net();");
                }
            }
        );

        logger.LogDebug("services.Logging();");
        logger.LogDebug("configureDefaults: {configureDefaults}", configureDefaults);
        if (configureDefaults)
        {
            services.Configure<DiginsightActivitiesOptions>(
                dao =>
                {
                    dao.LogBehavior = LogBehavior.Show;
                    dao.MeterName = assemblyName;
                }
            );

            services.AddSingleton<IConfigureClassAwareOptions<DiginsightActivitiesOptions>>(
                new ConfigureClassAwareOptions<DiginsightActivitiesOptions>(
                    Options.DefaultName,
                    static (t, dao) =>
                    {
                        IReadOnlyList<string> markers = ClassConfigurationMarkers.For(t);
                        if (markers.Contains("Diginsight.*"))
                        {
                            dao.RecordSpanDurations = true;
                        }
                    }
                )
            );
        }

        services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection(ConfigurationPath.Combine("Diginsight", "Activities")));
        services
            .VolatilelyConfigureClassAware<DiginsightActivitiesOptions>()
            .DynamicallyConfigureClassAware<DiginsightActivitiesOptions>();
        logger.LogDebug("services.ConfigureClassAware<DiginsightActivitiesOptions>();");

        IOpenTelemetryBuilder openTelemetryBuilder = services.AddDiginsightOpenTelemetry(); logger.LogDebug("services.AddDiginsightOpenTelemetry();");

        return services;
    }

}
