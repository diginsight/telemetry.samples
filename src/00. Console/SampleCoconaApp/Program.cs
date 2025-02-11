using Cocona;
using Cocona.Builder;
using Diginsight;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace SampleCoconaApp;

internal class Program
{
    internal static readonly ActivitySource ActivitySource = new(typeof(Program).Namespace!);

    private readonly ILogger<Program> logger;
    private readonly IServiceProvider serviceProvider;

    public Program(ILogger<Program> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    private static async Task Main(string[] args)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        CoconaAppBuilder appBuilder = CoconaApp.CreateBuilder(args);

        IConfiguration configuration = appBuilder.Configuration;
        IServiceCollection services = appBuilder.Services;

        appBuilder.Configuration.AddJsonFile("appsettings.json");

        // AddObservability()
        services.Configure<DiginsightActivitiesOptions>(configuration.GetSection("Diginsight:Activities"));
        services.AddLogging(
            lb => {
                lb.AddConfiguration(configuration);
                lb.ClearProviders();
                lb.AddDiginsightConsole(configuration.GetSection("Diginsight:Console").Bind);
            }
        );
        appBuilder.Host.UseDiginsightServiceProvider(true);

        services.AddSingleton<Executor>();
        services.AddSingleton<Program>();

        using CoconaApp app = appBuilder.Build();

        Executor executor = app.Services.GetRequiredService<Executor>();
        app.AddCommand(executor.ExecuteAsync);
        await app.RunAsync();
    }


}
