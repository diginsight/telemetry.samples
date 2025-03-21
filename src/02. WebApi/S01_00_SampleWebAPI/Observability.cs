using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Reflection;

namespace S01_00_SampleWebAPI;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
    public static ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;
}

