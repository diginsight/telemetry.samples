using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace B02_01_BlazorWebassemblyApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMicrosoftGraphClient("https://graph.microsoft.com/User.Read");

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
            });

            await builder.Build().RunAsync();
        }
    }
}
