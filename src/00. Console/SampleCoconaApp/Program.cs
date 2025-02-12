using Cocona;
using Cocona.Builder;
using Diginsight;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace SampleCoconaApp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        CoconaAppBuilder appBuilder = CoconaApp.CreateBuilder(args);

        IConfiguration configuration = appBuilder.Configuration;
        IServiceCollection services = appBuilder.Services;
        IHostEnvironment hostEnvironment = appBuilder.Environment;

        services.AddObservability(configuration, hostEnvironment);

        services.AddSingleton<Executor>();

        appBuilder.Host.UseDiginsightServiceProvider(true);
        using CoconaApp app = appBuilder.Build();

        Executor executor = app.Services.GetRequiredService<Executor>();
        app.AddCommand(executor.ExecuteAsync);
        await app.RunAsync();
    }
}
