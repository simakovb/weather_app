using Microsoft.AspNetCore.Mvc;
using WeatherApp.Configuration;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWeather([FromQuery] string unit = "metric")
        {
            var results = new List<Weather>();
            foreach (var city in CityConstants.Cities)
            {
                var weather = await _weatherService.GetWeatherAsync(city, unit);
                results.Add(weather);
            }

            return Ok(results);
        }

        [HttpGet("trend")]
        public async Task<IActionResult> GetTrends()
        {
            var trends = await _weatherService.GetTrendsAsync();
            return Ok(trends);
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast()
        {
            var forecast = await _weatherService.GenerateForecastAsync();
            return Ok(forecast);
        }
    }

}
