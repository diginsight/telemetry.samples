//using Microsoft.AspNetCore.Mvc;
////using System.Web.Http;

//namespace S10_00_AspnetWebApi480.Controllers;

//[ApiController]
//[Route("[controller]")]
//public class WeatherForecastController : ControllerBase
//{
//    private static readonly string[] Summaries = new[]
//    {
//        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//    };

//    private readonly ILogger<WeatherForecastController> _logger;

//    public WeatherForecastController(ILogger<WeatherForecastController> logger)
//    {
//        _logger = logger;
//    }

//    [HttpGet(Name = "GetWeatherForecast")]
//    public IEnumerable<WeatherForecast> Get()
//    {
//        var rnd = new Random();

//        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//        {
//            Date = DateTime.Now,
//            TemperatureC = rnd.Next(-20, 55),
//            Summary = Summaries[rnd.Next(Summaries.Length)]
//        })
//        .ToArray();
//    }
//}
