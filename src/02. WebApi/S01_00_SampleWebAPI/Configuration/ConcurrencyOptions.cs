using Diginsight.Options;

namespace S01_00_SampleWebAPI;

public class ConcurrencyOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public int? MaxConcurrency { get; set; }
}
