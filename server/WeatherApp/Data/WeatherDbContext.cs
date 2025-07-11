using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WeatherApp.DTOs;
using WeatherApp.Models;

namespace WeatherApp.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        public DbSet<WeatherData> WeatherHistory { get; set; }
    }
}
