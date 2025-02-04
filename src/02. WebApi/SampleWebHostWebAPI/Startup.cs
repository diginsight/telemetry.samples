using Diginsight;
using Diginsight.AspNetCore;
using Diginsight.Diagnostics;
using Diginsight.Stringify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        observabilityManager.AttachTo(services);

        services.AddHttpContextAccessor();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

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

        //services.AddControllers()
        //    .AddControllersAsServices()
        //    .ConfigureApiBehaviorOptions(opt =>
        //    {
        //        opt.SuppressModelStateInvalidFilter = true;
        //    })
        //    .AddJsonOptions(opt =>
        //    {
        //        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        //        opt.JsonSerializerOptions.WriteIndented = true;

        //        //opt.JsonSerializerOptions.PropertyNamingPolicy = new PascalCaseJsonNamingPolicy();
        //        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //    })
        //    .AddMvcOptions(opt =>
        //    {
        //        opt.MaxModelValidationErrors = 25;
        //        //opt.Conventions.Add(new DataExportConvention() as IControllerModelConvention);
        //        //opt.Conventions.Add(new DataExportConvention() as IActionModelConvention);
        //    });

        ////services.TryAddSingleton<IActivityTagger, ActivityTagger>();

        ////IsSwaggerEnabled = configuration.GetValue<bool>("IsSwaggerEnabled");
        ////if (IsSwaggerEnabled)
        ////{
        ////    services.AddSwaggerDocumentation();
        ////}
        ///

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

