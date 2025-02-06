
using Diginsight;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using log4net.Appender;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace SampleWebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

        WebApplication app;
        using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Add services to the container.
            services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection("Diginsight:Activities"));
            services.Configure<DiginsightConsoleFormatterOptions>(configuration.GetSection("Diginsight:Console"));
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
            observabilityManager.AttachTo(services);
            services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            builder.Host.UseDiginsightServiceProvider(true);

            app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }

        app.Run();
    }
}
