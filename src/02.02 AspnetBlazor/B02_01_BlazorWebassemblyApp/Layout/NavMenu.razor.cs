using B02_01_BlazorWebassemblyApp.Configuration;
using Microsoft.AspNetCore.Components;

namespace B02_01_BlazorWebassemblyApp.Layout;

public partial class NavMenu
{
    [Inject]
    internal ClientAuthBootstrapState AuthBootstrapState { get; set; } = default!;

    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
