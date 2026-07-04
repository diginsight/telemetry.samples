using B02_01_BlazorWebassemblyApp.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace B02_01_BlazorWebassemblyApp.Layout;

public partial class LoginDisplay
{
    [Inject]
    internal ClientAuthBootstrapState AuthBootstrapState { get; set; } = default!;

    public void BeginLogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
}
