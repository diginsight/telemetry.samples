using Diginsight.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;

namespace B02_02_BlazorWebassemblyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class WeatherForecastController : ControllerBase
    {
        ILogger<WeatherForecastController>  logger;
        private readonly GraphServiceClient graphServiceClient;
        public static Type CType = typeof(WeatherForecastController);

        public WeatherForecastController(ILogger<WeatherForecastController> logger, GraphServiceClient graphServiceClient)
        {
            this.logger = logger;
            this.graphServiceClient = graphServiceClient;
        }

        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger);

            var user = await graphServiceClient.Me.Request().GetAsync();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
