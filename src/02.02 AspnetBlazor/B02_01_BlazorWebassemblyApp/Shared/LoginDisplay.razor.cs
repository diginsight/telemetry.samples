using Microsoft.AspNetCore.Components.Web;

namespace B02_01_BlazorWebassemblyApp.Shared;

public partial class LoginDisplay
{
    private async Task BeginLogout(MouseEventArgs args)
    {
        await SignOutManager.SetSignOutState();
        Navigation.NavigateTo("authentication/logout");
    }
}
