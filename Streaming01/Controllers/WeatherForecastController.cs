using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Streaming01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetWeatherForecast1")]
        public IEnumerable<WeatherForecast> Get1()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet("GetWeatherForecast2")]
        public IAsyncEnumerable<WeatherForecast> Get2()
        {
            return FetchItems();
        }

        static async IAsyncEnumerable<WeatherForecast> FetchItems()
        {
            for (int i = 1; i <= 10; i++)
            {
                //await Task.Delay(1000);
                yield return new WeatherForecast
                {  
                    Date = DateTime.Now.AddDays(i),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };
            }
        }

        /// <summary>
        /// This is the second best exmaple to follow
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetWeatherForecast")]
        public async IAsyncEnumerable<WeatherForecast> Get()
        {
            for (int i = 1; i <= 20; i++)
            {
                await Task.Delay(3 * 1000);
                yield return new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(i),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };
            }
        }


        /// <summary>
        /// This is the example to follow for streaming results back to UI
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStreamingData")]
        public async Task<IActionResult> GetStreamingData()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            Response.ContentType = "application/json";

            StreamWriter streamWriter;
            await using ((streamWriter = new StreamWriter(Response.Body)).ConfigureAwait(false))
            {
                await foreach (var weatherForecast in FetchItems())
                {
                    var stringWeatherForecast = System.Text.Json.JsonSerializer.Serialize(weatherForecast);
                    await streamWriter.WriteLineAsync(stringWeatherForecast).ConfigureAwait(false);
                    await streamWriter.FlushAsync().ConfigureAwait(false);
                    
                    //await Task.Delay(3 * 1000);
                }
            }

            return new EmptyResult();
        }

        private IEnumerable<WeatherForecast> GetData()
        {
            for (int i = 1; i <= 20; i++)
            {
                yield return new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(i),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };
            }
        }

    }
}