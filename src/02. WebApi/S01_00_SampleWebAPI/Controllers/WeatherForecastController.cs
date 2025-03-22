using Diginsight.Diagnostics;
using Diginsight.Options;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace S01_00_SampleWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	private static readonly string[] Summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
	private readonly ILogger<WeatherForecastController> logger;
	private readonly IClassAwareOptionsMonitor<ConcurrencyOptions> concurrencyOptionsMonitor;

	public WeatherForecastController(ILogger<WeatherForecastController> logger,
		IClassAwareOptionsMonitor<ConcurrencyOptions> concurrencyOptionsMonitor)
	{
		this.logger = logger;
		this.concurrencyOptionsMonitor = concurrencyOptionsMonitor;

		using var activity = Observability.ActivitySource.StartMethodActivity(logger);

	}

	[HttpGet(Name = "GetWeatherForecast")]
	public async Task<IEnumerable<WeatherForecast>> Get()
	{
		using var activity = Observability.ActivitySource.StartMethodActivity(logger);

		var maxConcurrency = concurrencyOptionsMonitor.CurrentValue?.MaxConcurrency ?? -1; logger.LogDebug("maxConcurrency: {maxConcurrency}", maxConcurrency);
		var options = new ParallelOptions() { MaxDegreeOfParallelism = maxConcurrency };

		int[] ia = new int[20];
		int index = 0;
		var queue = new ConcurrentQueue<WeatherForecast>();
		await Parallel.ForEachAsync(ia, options, async (i, ct) =>
		{
			index++;
			var randomTemperature = Random.Shared.Next(-20, 55);
			logger.LogDebug("index {index}, randomTemperature: {randomTemperature}", index, randomTemperature);
			var weatherForecast = new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = randomTemperature,
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			};

			queue.Enqueue(weatherForecast);
		});

		var res = queue.ToArray();
		activity?.SetOutput(res);
		return res;
	}
}
