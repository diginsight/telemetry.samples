using System.Diagnostics;
using System.Reflection;

namespace SampleWebAPIWithStartupSequence;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
    public static ILoggerFactory LoggerFactory { get; set; }
}

