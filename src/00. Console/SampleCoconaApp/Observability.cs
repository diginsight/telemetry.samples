using System.Diagnostics;
using System.Reflection;

namespace SampleCoconaApp;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
}

