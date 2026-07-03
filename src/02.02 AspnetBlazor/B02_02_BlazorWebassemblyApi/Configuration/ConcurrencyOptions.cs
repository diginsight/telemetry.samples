using Diginsight.Options;

namespace B02_02_BlazorWebassemblyApi;

public class ConcurrencyOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public int? MaxConcurrency { get; set; }
}
