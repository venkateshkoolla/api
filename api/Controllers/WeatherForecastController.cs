using System;
using System.Collections.Generic;
using System.Linq;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WeatherForecastController : ControllerBase
    {
        private SharedMemory _sharedMemory;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, SharedMemory sharedMemory)
        {
            _logger = logger;
            _sharedMemory = sharedMemory;

        }

        // https://localhost:44395/weatherforecast/
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<WeatherForecast>> All()
        {
            return _sharedMemory.weatherForecasts;
        }

        //https://localhost:44395/weatherforecast
        [HttpPut]
        [Authorize]
        public ActionResult<WeatherForecast> Put(WeatherForecast model)
        {
            var wfc = new WeatherForecast()
            {
                City = model.City,
                details = new List<Detail>()
            };

            var isExists = _sharedMemory.weatherForecasts.Any(x => x.City == model.City);
            if (!isExists)
            {
                foreach (var detail in model.details)
                {
                    wfc.details.Add(detail);
                }
                _sharedMemory.weatherForecasts.Add(wfc);
            }
            else
            {
                var item = _sharedMemory.weatherForecasts.FirstOrDefault(x => x.City == model.City);
                var detailsNotExist = model.details.Where(x => !item.details.Any(y => y.Date == x.Date));

                var updatedDetail = item
                     .details
                     .Concat(detailsNotExist);
                _sharedMemory.weatherForecasts.FirstOrDefault(x => x.City == model.City).details = updatedDetail.ToList();
            }

            return _sharedMemory.weatherForecasts.FirstOrDefault(x => x.City == model.City);
        }

        //https://localhost:44395/weatherforecast/{city}
        [HttpGet("{city}")]
        [Authorize]
        public ActionResult<WeatherForecast> Get(string city)
        {
            return _sharedMemory.weatherForecasts
                .FirstOrDefault(x => string.Compare(x.City, city, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        // https://localhost:44395/weatherforecast/vancouver
        [HttpPost("{city}")]
        [Authorize]
        public ActionResult<IEnumerable<WeatherForecast>> Delete(string city)
        {
            var item = _sharedMemory.weatherForecasts
                .FirstOrDefault(x => string.Compare(x.City, city, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (item != null)
            {
                _sharedMemory.weatherForecasts.Remove(item);
                return _sharedMemory.weatherForecasts;
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
