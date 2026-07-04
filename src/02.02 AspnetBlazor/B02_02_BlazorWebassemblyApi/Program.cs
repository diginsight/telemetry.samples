using Diginsight;
using Diginsight.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.OpenApi;
using B02_02_BlazorWebassemblyApi.Routing;

namespace B02_02_BlazorWebassemblyApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var observabilityManager = new ObservabilityManager();
            //ObservabilityRegistry.RegisterLoggerFactory(observabilityManager.LoggerFactory);
            LoggerFactoryStaticAccessor.LoggerFactory = observabilityManager.LoggerFactory;

            ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

            WebApplication app;
            using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
            {
                var builder = WebApplication.CreateBuilder(args);

                var services = builder.Services;
                var configuration = builder.Configuration;
                var environment = builder.Environment;

                // Add logging and opentelemetry
                services.AddObservability(configuration, environment);
                observabilityManager.AttachTo(services);

                // Add services to the container.
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
                        .EnableTokenAcquisitionToCallDownstreamApi()
                        .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
                        .AddInMemoryTokenCaches();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("BlazorClient", policy =>
                    {
                        policy
                            .WithOrigins("https://localhost:7123", "http://localhost:5241")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });

                builder.Services.AddControllers(options =>
                {
                    options.Conventions.Add(new RouteTokenTransformerConvention(new LowercaseParameterTransformer()));
                });
                //services.AddEndpointsApiExplorer();
                //services.AddSwaggerGen();


                // Azure AD values used to wire up the Swagger "Authorize" (OAuth2) flow
                var azureAd = configuration.GetSection("AzureAd");
                var instance = (azureAd["Instance"] ?? "https://login.microsoftonline.com/").TrimEnd('/');
                var tenantId = azureAd["TenantId"];
                var apiClientId = azureAd["ClientId"];
                var scopeName = azureAd["Scopes"] ?? "access_as_user";
                var apiScope = $"api://{apiClientId}/{scopeName}";
                var authorizationUrl = new Uri($"{instance}/{tenantId}/oauth2/v2.0/authorize");
                var tokenUrl = new Uri($"{instance}/{tenantId}/oauth2/v2.0/token");

                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi(options =>
                {
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        var oauthScheme = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Description = "Azure AD OAuth2 (Authorization Code + PKCE)",
                            Flows = new OpenApiOAuthFlows
                            {
                                AuthorizationCode = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = authorizationUrl,
                                    TokenUrl = tokenUrl,
                                    Scopes = new Dictionary<string, string>
                                    {
                                        [apiScope] = "Access the API as the signed-in user"
                                    }
                                }
                            }
                        };

                        document.Components ??= new OpenApiComponents();
                        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                        document.Components.SecuritySchemes["oauth2"] = oauthScheme;

                        var schemeReference = new OpenApiSecuritySchemeReference("oauth2", document);
                        document.Security ??= new List<OpenApiSecurityRequirement>();
                        document.Security.Add(new OpenApiSecurityRequirement
                        {
                            [schemeReference] = new List<string> { apiScope }
                        });

                        return Task.CompletedTask;
                    });
                });

                builder.Host.UseDiginsightServiceProvider(true);
                app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsProduction())
                {
                    app.MapOpenApi();

                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/openapi/v1.json", "B02_02_BlazorWebassemblyApi v1");
                        options.OAuthClientId(apiClientId);
                        options.OAuthAppName("B02_02_BlazorWebassemblyApi - Swagger");
                        options.OAuthUsePkce();
                        options.OAuthScopeSeparator(" ");
                        options.OAuthScopes(apiScope);
                    });
                }

                app.UseHttpsRedirection();

                app.UseCors("BlazorClient");

                app.UseAuthentication();

                app.UseAuthorization();

                app.MapControllers();
            }

            //services.ConfigureClassAware<ConcurrencyOptions>(configuration.GetSection("AppSettings"))
            //    .DynamicallyConfigureClassAware<ConcurrencyOptions>()
            //    .VolatilelyConfigureClassAware<ConcurrencyOptions>();

            app.Run();
        }
    }
}
