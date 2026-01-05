
using Diginsight;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using log4net.Appender;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace SampleWebAPIDocker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var observabilityManager = new ObservabilityManager();
            // ObservabilityRegistry.RegisterLoggerFactory(observabilityManager.LoggerFactory);
            ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

            WebApplication app;
            using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
            {
                var builder = WebApplication.CreateBuilder(args); logger.LogDebug("Creating WebApplication builder");
                var services = builder.Services;
                var configuration = builder.Configuration;
                string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;

                services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection("Diginsight:Activities")); logger.LogDebug("Configuring DiginsightActivitiesOptions from Diginsight:Activities");
                services.Configure<DiginsightConsoleFormatterOptions>(configuration.GetSection("Diginsight:Console")); logger.LogDebug("Configuring DiginsightConsoleFormatterOptions from Diginsight:Console");
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
                                    loggingBuilder.AddDiginsightLog4Net(sp =>
                                    {
                                        IHostEnvironment env = sp.GetRequiredService<IHostEnvironment>();
                                        string fileBaseDir = env.IsDevelopment()
                                                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify)
                                                : $"{Path.DirectorySeparatorChar}home";

                                        return new IAppender[]
                                        {
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
                                        };
                                    },
                                    static _ => log4net.Core.Level.All);
                                }
                            }
                        ); logger.LogDebug("services.AddLogging()");

                services.TryAddSingleton<IActivityLoggingFilter, OptionsBasedActivityLoggingFilter>(); logger.LogDebug("services.TryAddSingleton<IActivityLoggingFilter, OptionsBasedActivityLoggingFilter>();");
                observabilityManager.AttachTo(services); logger.LogDebug("observabilityManager.AttachTo(services)");

                builder.Services.AddControllers(); logger.LogDebug("builder.Services.AddControllers()");
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer(); logger.LogDebug("builder.Services.AddEndpointsApiExplorer()");
                builder.Services.AddSwaggerGen(); logger.LogDebug("builder.Services.AddSwaggerGen()");

                builder.Host.UseDiginsightServiceProvider(true); logger.LogDebug("builder.Host.UseDiginsightServiceProvider(true)");
                app = builder.Build(); logger.LogDebug("app = builder.Build()");

                logger.LogDebug("IsDevelopment: {IsDevelopment}", app.Environment.IsDevelopment());
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger(); logger.LogDebug("app.UseSwagger()");
                    app.UseSwaggerUI(); logger.LogDebug("app.UseSwaggerUI()");
                }

                app.UseHttpsRedirection(); logger.LogDebug("app.UseHttpsRedirection()");

                app.UseAuthorization(); logger.LogDebug("app.UseAuthorization()");

                app.MapControllers(); logger.LogDebug("app.MapControllers()");
            }

            logger.LogDebug("app.Run()");
            app.Run();
        }
    }
}
