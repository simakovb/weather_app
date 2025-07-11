using Microsoft.AspNetCore.Mvc;
using WeatherApp.Data;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("unit")]
    public class UnitController : ControllerBase
    {
        private static string _unit = "metric";

        [HttpPost]
        public IActionResult SetUnit([FromBody] UnitPreference model)
        {
            if (model.Unit is not ("metric" or "imperial"))
                return BadRequest("Invalid unit");

            _unit = model.Unit;
            return Ok(new { message = $"Unit set to {_unit}" });
        }

        [HttpGet]
        public IActionResult GetUnit() => Ok(new { unit = _unit });
    }

}
