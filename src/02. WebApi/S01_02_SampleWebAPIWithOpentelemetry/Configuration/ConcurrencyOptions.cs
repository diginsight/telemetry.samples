using Diginsight.Options;

namespace S01_02_SampleWebAPIWithOpentelemetry;

public class ConcurrencyOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public int? MaxConcurrency { get; set; }
}
