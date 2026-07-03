using Microsoft.Graph.Models;

namespace B02_01_BlazorWebassemblyApp.Pages;

public partial class UserProfile
{
    private User? user;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            user = await GraphServiceClient.Me.GetAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
