using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace SampleConsoleApp;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
}

