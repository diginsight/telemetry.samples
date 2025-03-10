
using Diginsight;
using Diginsight.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SampleWebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        // Add logging and opentelemetry
        services.AddObservability(configuration, environment);

        services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();

        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        builder.Host.UseDiginsightServiceProvider(true);
        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
