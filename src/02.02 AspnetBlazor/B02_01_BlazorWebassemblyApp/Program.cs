using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;
using B02_01_BlazorWebassemblyApp.Configuration;
using B02_02_BlazorWebassemblyModel;

namespace B02_01_BlazorWebassemblyApp
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var serverConfigBaseUrl = builder.Configuration["ServerConfig:BaseUrl"];
            var authEndpoint = builder.Configuration["ServerConfig:AuthEndpoint"] ?? "api/clientconfig/auth";
            var serverAuthConfig = await TryLoadServerAuthConfigAsync(serverConfigBaseUrl, authEndpoint);

            var authBootstrapState = new ClientAuthBootstrapState
            {
                IsConfigLoaded = serverAuthConfig is not null,
                IsAuthenticationConfigured = !string.IsNullOrWhiteSpace(serverAuthConfig?.ClientId)
            };

            builder.Services.AddSingleton(authBootstrapState);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMicrosoftGraphClient("https://graph.microsoft.com/User.Read");

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

                if (!string.IsNullOrWhiteSpace(serverAuthConfig?.ClientId))
                {
                    options.ProviderOptions.Authentication.ClientId = serverAuthConfig.ClientId;
                }

                if (!string.IsNullOrWhiteSpace(serverAuthConfig?.Authority))
                {
                    options.ProviderOptions.Authentication.Authority = serverAuthConfig.Authority;
                }

                if (serverAuthConfig?.ValidateAuthority is not null)
                {
                    options.ProviderOptions.Authentication.ValidateAuthority = serverAuthConfig.ValidateAuthority.Value;
                }

                if (string.IsNullOrWhiteSpace(options.ProviderOptions.Authentication.ClientId))
                {
                    // Keep app startup non-blocking; auth-only features remain disabled in UI until config is available.
                    options.ProviderOptions.Authentication.ClientId = "auth-not-configured";
                }

                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
            });

            await builder.Build().RunAsync();
        }

        private static async Task<ClientAuthConfigResponse?> TryLoadServerAuthConfigAsync(string? serverConfigBaseUrl, string authEndpoint)
        {
            if (string.IsNullOrWhiteSpace(serverConfigBaseUrl))
            {
                return null;
            }

            try
            {
                using var configurationHttpClient = new HttpClient { BaseAddress = new Uri(serverConfigBaseUrl) };
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                return await configurationHttpClient.GetFromJsonAsync<ClientAuthConfigResponse>(authEndpoint, cancellationTokenSource.Token);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                return null;
            }
        }
    }
}
