using System.Diagnostics;
using System.Reflection;

namespace SampleWebHostWebAPI;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
}

