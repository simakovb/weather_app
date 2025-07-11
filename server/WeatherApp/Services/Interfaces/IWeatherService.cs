using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<Weather> GetWeatherAsync(string city, string unit);
        Task<Dictionary<string, string>> GetTrendsAsync();
        Task<Dictionary<string, string>> GenerateForecastAsync();
    }

}
