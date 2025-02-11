using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using Diginsight.Options;
using log4net.Appender;
using log4net.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.IO;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Diginsight;

namespace SampleCoconaApp;

using Options = Microsoft.Extensions.Options.Options;

public static partial class ObservabilityExtensions
{
    static Type T = typeof(ObservabilityExtensions);

  

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment
    )
    {
        var loggerFactory = Observability.LoggerFactory;
        var logger = loggerFactory.CreateLogger(T);
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { services, configuration, hostEnvironment });

        const string diginsightConfKey = "Diginsight";
        const string observabilityConfKey = "Observability";

        bool isLocal = hostEnvironment.IsDevelopment(); logger.LogDebug("isLocal: {isLocal}", isLocal);
        string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!; logger.LogDebug("assemblyName: {assemblyName}", assemblyName);



        return services;
    }

}
