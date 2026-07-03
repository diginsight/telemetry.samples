namespace B02_01_BlazorWebassemblyApp.Shared;

public partial class RedirectToLogin
{
    protected override void OnInitialized()
    {
        Navigation.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(Navigation.Uri)}");
    }
}
