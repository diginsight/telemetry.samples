using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace B02_01_BlazorWebassemblyApp.Layout;

public partial class LoginDisplay
{
    public void BeginLogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
}
