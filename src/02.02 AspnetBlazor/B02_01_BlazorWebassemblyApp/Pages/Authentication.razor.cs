using Microsoft.AspNetCore.Components;

namespace B02_01_BlazorWebassemblyApp.Pages;

public partial class Authentication
{
    [Parameter]
    public string? Action { get; set; }
}
