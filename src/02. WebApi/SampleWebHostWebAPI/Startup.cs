using Diginsight;
using Diginsight.AspNetCore;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using Diginsight.Stringify;
using log4net.Appender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Text.Json.Serialization;

namespace SampleWebHostWebAPI;

public class Startup
{
    private readonly IConfiguration configuration;
    private readonly IHostEnvironment hostEnvironment;
    private readonly EarlyLoggingManager observabilityManager;
    private readonly ILoggerFactory loggerFactory;

    public Startup(IConfiguration configuration,
                   IHostEnvironment hostEnvironment,
                   EarlyLoggingManager observabilityManager)
    {
        this.configuration = configuration;
        this.hostEnvironment = hostEnvironment;
        this.observabilityManager = observabilityManager;
        loggerFactory = observabilityManager.LoggerFactory;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var logger = loggerFactory.CreateLogger<Startup>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { services });

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

        services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();
        observabilityManager.AttachTo(services);

        //services.AddHttpContextAccessor();
        //services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // services.AddDiginsightOpenTelemetry().WithTracing(b => b.SetSampler(new AlwaysOnSampler()));
        // services.TryAddEnumerable(ServiceDescriptor.Singleton<IActivityListenerRegistration, ControllerActivityTaggerRegistration>());
        services.AddDynamicLogLevel<DefaultDynamicLogLevelInjector>();

        // configure type contracts for log string rendering
        static void ConfigureTypeContracts(StringifyTypeContractAccessor accessor)
        {
            //accessor.GetOrAdd<RestResponse>(
            //    static typeContract =>
            //    {
            //        typeContract.GetOrAdd(static x => x.Request, static mc => mc.Included = false);
            //        typeContract.GetOrAdd(static x => x.ResponseStatus, static mc => mc.Order = 1);
            //        //typeContract.GetOrAdd(static x => x.Content, static mc => mc.Order = 1);
            //    }
            //);
        }
        StringifyContextFactoryBuilder.DefaultBuilder.ConfigureContracts(ConfigureTypeContracts);
        services.Configure<StringifyTypeContractAccessor>(ConfigureTypeContracts);



        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var logger = loggerFactory.CreateLogger<Startup>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { app, env });

        if (env.IsDevelopment())
        {
            //IdentityModelEventSource.ShowPII = true;
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        //app.UseCors();

        //app.MapControllers();

        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapVolatileConfiguration();
            endpoints.MapControllers();
        });

    }

    private bool IsSwaggerEnabled { get; set; }
}

