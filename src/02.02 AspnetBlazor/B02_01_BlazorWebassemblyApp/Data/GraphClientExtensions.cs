using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

using BlazorAccessTokenProvider = Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider;

/// <summary>
/// Adds services and implements methods to use Microsoft Graph SDK.
/// </summary>
internal static class GraphClientExtensions
{
    /// <summary>
    /// Extension method for adding the Microsoft Graph SDK to IServiceCollection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="scopes">The MS Graph scopes to request</param>
    /// <returns></returns>
    public static IServiceCollection AddMicrosoftGraphClient(this IServiceCollection services, params string[] scopes)
    {
        services.Configure<RemoteAuthenticationOptions<MsalProviderOptions>>(options =>
        {
            foreach (var scope in scopes)
            {
                options.ProviderOptions.AdditionalScopesToConsent.Add(scope);
            }
        });

        services.AddScoped<IAuthenticationProvider>(sp =>
        {
            var tokenProvider = new GraphTokenProvider(sp.GetRequiredService<BlazorAccessTokenProvider>());
            return new BaseBearerTokenAuthenticationProvider(tokenProvider);
        });
        services.AddScoped(sp => new GraphServiceClient(
              new HttpClient(),
              sp.GetRequiredService<IAuthenticationProvider>()));
        return services;
    }

    /// <summary>
    /// Implements Kiota IAccessTokenProvider interface.
    /// Tries to get an access token for Microsoft Graph.
    /// </summary>
    private class GraphTokenProvider : Microsoft.Kiota.Abstractions.Authentication.IAccessTokenProvider
    {
        private readonly BlazorAccessTokenProvider _provider;

        public GraphTokenProvider(BlazorAccessTokenProvider provider)
        {
            _provider = provider;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator(new[] { "graph.microsoft.com" });

        public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            var result = await _provider.RequestAccessToken(new AccessTokenRequestOptions()
            {
                Scopes = new[] { "https://graph.microsoft.com/User.Read" }
            });

            if (result.TryGetToken(out var token))
            {
                return token.Value;
            }

            return string.Empty;
        }
    }
}
