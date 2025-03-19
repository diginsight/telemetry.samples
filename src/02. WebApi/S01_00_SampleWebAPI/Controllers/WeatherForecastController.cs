using Diginsight.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SampleWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
    private readonly ILogger<WeatherForecastController> logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        this.logger = logger;
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        var randomTemperature = Random.Shared.Next(-20, 55);
        logger.LogDebug("randomTemperature: {randomTemperature}", randomTemperature);

        var res = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = randomTemperature,
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();

        activity?.SetOutput(res);
        return res;
    }
}
