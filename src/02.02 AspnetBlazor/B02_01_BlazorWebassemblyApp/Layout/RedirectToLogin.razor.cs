using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace B02_01_BlazorWebassemblyApp.Layout;

public partial class RedirectToLogin
{
    protected override void OnInitialized()
    {
        Navigation.NavigateToLogin("authentication/login");
    }
}
