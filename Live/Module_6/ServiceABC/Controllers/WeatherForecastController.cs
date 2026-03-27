using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ServiceABC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private static readonly List<WeatherForecast> _forecasts;

        static WeatherForecastController()
        {
            _forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToList();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [Authorize(Roles ="MagLezen")]
        public IEnumerable<WeatherForecast> Get()
        {
            foreach(var claim in HttpContext.User.Claims)
            {
                Console.WriteLine($"{claim.Subject.Name}: {claim.Value}");
            }

            return _forecasts;
        }

        [HttpPost(Name = "PostWeatherForecast")]
        public IActionResult Post([FromBody]WeatherForecast forecast)
        {
            _forecasts.Add(forecast);
            return Accepted(forecast);

        }
    }
}
