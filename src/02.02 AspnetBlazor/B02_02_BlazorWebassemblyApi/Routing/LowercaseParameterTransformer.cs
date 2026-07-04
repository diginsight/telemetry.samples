using Microsoft.AspNetCore.Routing;

namespace B02_02_BlazorWebassemblyApi.Routing
{
    public sealed class LowercaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value?.ToString()?.ToLowerInvariant();
        }
    }
}
