namespace B02_02_BlazorWebassemblyModel
{
    public sealed class ClientAuthConfigResponse
    {
        public string? ClientId { get; set; }

        public string? Authority { get; set; }

        public bool? ValidateAuthority { get; set; }
    }
}
