
using Diginsight.Diagnostics;
using Diginsight;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SampleWebAPIWithOpentelemetry;

namespace SampleWebAPIWithOpentelemetry;

public class Program
{
    public static void Main(string[] args)
    {
        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));
        Observability.LoggerFactory = observabilityManager.LoggerFactory;

        WebApplication app;
        using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;
            var environment = builder.Environment;

            // Add logging and opentelemetry
            services.AddObservability(configuration, environment, out IOpenTelemetryOptions openTelemetryOptions);

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
