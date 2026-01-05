using Diginsight;
using Diginsight.AspNetCore;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using Diginsight.Options;
using log4net.Appender;
using log4net.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace SampleWebAPI;

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
        bool isLocal = hostEnvironment.IsDevelopment(); 
        string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!; 

        services.TryAddSingleton<IActivityLoggingFilter, OptionsBasedActivityLoggingFilter>();

        services.AddLogging(
            loggingBuilder =>
            {
                loggingBuilder.ClearProviders(); 

                var consoleEnabled = configuration.GetValue(ConfigurationPath.Combine("Observability", "ConsoleEnabled"), true); 
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
                }

                var log4NetEnabled = configuration.GetValue(ConfigurationPath.Combine("Observability", "Log4NetEnabled"), true); 
                if (log4NetEnabled)
                {
                    loggingBuilder.AddDiginsightLog4Net(
                        sp =>
                        {
                            IHostEnvironment env = sp.GetRequiredService<IHostEnvironment>();
                            string fileBaseDir = env.IsDevelopment()
                                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify)
                                : $"{Path.DirectorySeparatorChar}home";

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
                }
            }
        );

        if (configureDefaults)
        {
            services.Configure<DiginsightActivitiesOptions>(
                dao =>
                {
                    dao.LogBehavior = LogBehavior.Show;
                    dao.RecordSpanDuration = true;  // Was RecordSpanDurations
                    dao.SpanDurationMeterName = assemblyName;  // Was MeterName
                    dao.SpanDurationMetricName = "diginsight.span_duration";  // Was MetricName
                    dao.SpanDurationMetricDescription = "Duration of application spans";  // Was MetricDescription
                                                                                          // dao.MetricUnit removed
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
                            dao.RecordSpanDuration = true;
                        }
                    }
                )
            );
        }

        services.AddHttpContextAccessor();
        DefaultDynamicConfigurationLoader.AddToServices(services);
        services.AddDynamicLogLevel<DefaultDynamicLogLevelInjector>();

        services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection(ConfigurationPath.Combine("Diginsight", "Activities")));
        services
            .VolatilelyConfigureClassAware<DiginsightActivitiesOptions>()
            .DynamicallyConfigureClassAware<DiginsightActivitiesOptions>();

        services.TryAddSingleton<IActivityLoggingFilter, OptionsBasedActivityLoggingFilter>();

        return services;
    }

}
