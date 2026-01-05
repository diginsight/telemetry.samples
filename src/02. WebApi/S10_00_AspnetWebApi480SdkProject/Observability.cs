using System.Diagnostics;
using System.Reflection;

namespace S10_00_AspnetWebApi480SdkProject;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
}

