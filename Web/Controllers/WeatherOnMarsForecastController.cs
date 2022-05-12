using Microsoft.AspNetCore.Mvc;

namespace api_versioning_828_repro.Controllers;

[Route("[controller]")]
public class WeatherOnMarsForecastController : BaseApiController
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherOnMarsForecastController> _logger;

    public WeatherOnMarsForecastController(ILogger<WeatherOnMarsForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet("today")]
    public ActionResult GetWeatherToday()
    {
        return Ok(new
        {
            Type = "Today",
            Temps = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray(),
        });

    }

    [HttpGet("yesterday")]
    public ActionResult GetWeatherFromYesterday()
    {
        return Ok(new
        {
            Type = "Yesterday",
            Temps = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray(),
        });
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("tomorrow")]
    // REPRO: No ApiVersion is specified, but CustomApiVersionSelector still receives 2.0 in its model
    public ActionResult GetWeatherTomorrow()
    {
        return Ok(new
        {
            Type = "Tomorrow",
            Temps = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray(),
        });
    }
}
