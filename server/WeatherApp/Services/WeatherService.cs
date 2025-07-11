using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WeatherApp.Configuration;
using WeatherApp.Data;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IMemoryCache _cache;
        private readonly WeatherDbContext _db;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(IMemoryCache cache, IHttpClientFactory clientFactory, WeatherDbContext db, IConfiguration configuration)
        {
            _cache = cache;
            _httpClient = clientFactory.CreateClient("OpenWeather");
            _db = db;
            _apiKey = configuration["WeatherService:ApiKey"];
        }

        public async Task<Dictionary<string, string>> GenerateForecastAsync()
        {
            var result = new Dictionary<string, string>();
            var forecastHours = new int[] { 1, 2, 3 };

            foreach (var city in CityConstants.Cities)
            {
                var recentData = await _db.WeatherHistory
                    .Where(w => w.City == city)
                    .OrderByDescending(w => w.Timestamp)
                    .Take(5)
                    .Select(w => new { w.Temperature, w.Timestamp })
                    .ToListAsync();

                if (recentData.Count < 2)
                {
                    result[city] = "Insufficient data for forecast";
                    continue;
                }

                double totalDeltaTemp = 0;
                double totalDeltaHours = 0;

                for (int i = 0; i < recentData.Count - 1; i++)
                {
                    var tempDiff = recentData[i].Temperature - recentData[i + 1].Temperature;
                    var timeDiff = (recentData[i].Timestamp - recentData[i + 1].Timestamp).TotalHours;

                    if (timeDiff > 0)
                    {
                        totalDeltaTemp += tempDiff;
                        totalDeltaHours += timeDiff;
                    }
                }

                if (totalDeltaHours == 0)
                {
                    result[city] = "No time difference in data to forecast";
                    continue;
                }

                var avgTempChangePerHour = totalDeltaTemp / totalDeltaHours;
                var latestTemp = recentData[0].Temperature;

                var forecasts = forecastHours.Select(h =>
                    $"{h}h: {Math.Round(latestTemp + avgTempChangePerHour * h, 1)}°");

                result[city] = string.Join(", ", forecasts);
            }

            return result;
        }


        public async Task<Weather> GetWeatherAsync(string city, string unit)
        {
            string cacheKey = $"{city}_{unit}";
            if (_cache.TryGetValue(cacheKey, out Weather cached))
                return cached;

            var url = $"/data/2.5/weather?q={city}&appid={_apiKey}&units={unit}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Weather API error: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<Weather>();

            if (result is null)
                throw new Exception("Empty or invalid weather response");

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            double tempCelsius = unit == "imperial"
                ? (result.Main.Temp - 32) * 5 / 9
                : result.Main.Temp;

            _db.WeatherHistory.Add(new WeatherData
            {
                City = city,
                Temperature = tempCelsius,
                Timestamp = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return result;
        }

        public async Task<Dictionary<string, string>> GetTrendsAsync()
        {
            var result = new Dictionary<string, string>();

            foreach (var city in CityConstants.Cities)
            {
                var records = await _db.WeatherHistory
                    .Where(w => w.City == city)
                    .OrderByDescending(w => w.Timestamp)
                    .Take(10)
                    .Select(w => new { w.Temperature, w.Timestamp })
                    .ToListAsync();

                if (records.Count < 2)
                {
                    result[city] = "insufficient data";
                    continue;
                }

                var trendType = GetTrendType(records[0].Temperature, records[1].Temperature);
                if (trendType == "stable")
                {
                    result[city] = "stable";
                    continue;
                }

                var baseTime = records[0].Timestamp;
                DateTime trendEndTime = records[0].Timestamp;

                for (int i = 1; i < records.Count; i++)
                {
                    var currentTrend = GetTrendType(records[i - 1].Temperature, records[i].Temperature);
                    if (currentTrend != trendType)
                        break;

                    trendEndTime = records[i].Timestamp;
                }

                var hours = (int)Math.Floor((baseTime - trendEndTime).TotalHours);
                if (hours < 1)
                    hours = 1;

                result[city] = $"{trendType} for {hours} hour{(hours > 1 ? "s" : "")}";
            }

            return result;
        }

        private string GetTrendType(double t1, double t2)
        {
            var delta = t1 - t2;
            if (Math.Abs(delta) < 0.5) return "stable";
            return delta > 0 ? "rising" : "falling";
        }

    }

}
