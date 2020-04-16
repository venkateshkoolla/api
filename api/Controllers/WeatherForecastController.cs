using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
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

        // https://localhost:{port}/weatherforecast/
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<WeatherForecast>> All()
        {
            return _sharedMemory.weatherForecasts;
        }

        //https://localhost:{port}/weatherforecast
        [HttpPut]
        [Authorize]
        public ActionResult<WeatherForecast> Put(WeatherForecast model)
        {
            if (model == null) throw new ValidationException("request body can not be empty.");

            var wfc = new WeatherForecast()
            {
                City = model.City,
                details = new List<Detail>()
            };

            try
            {
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
                    List<Detail> updatedDetails;

                    // Added extra weather days from request
                    if (item.details.Count < model.details.Count)
                    {

                        updatedDetails = model.details.Where(x => !item.details.Any(y => y.Date == x.Date)).ToList();
                        var updatedDetail = item
                         .details
                         .Concat(updatedDetails);
                        _sharedMemory.weatherForecasts.FirstOrDefault(x => x.City == model.City).details = updatedDetail.ToList();
                    }

                    // removed existing weather days from request
                    else if (item.details.Count > model.details.Count)
                    {
                        updatedDetails = item.details.Where(x => !model.details.Any(y => y.Date == x.Date)).ToList();
                        for (int i = item.details.Count - 1; i > -1; i--)
                        {
                            Detail detail = item.details[i];
                            for (int j = 0; j <= updatedDetails.Count() - 1; j++)
                            {
                                Detail removedDetail = updatedDetails[j];
                                if (detail.Date == removedDetail.Date)
                                {
                                    item.details.RemoveAt(i);
                                }
                            }
                        }
                        _sharedMemory.weatherForecasts.FirstOrDefault(x => x.City == model.City).details = item.details;
                    }
                }

                return _sharedMemory.weatherForecasts.FirstOrDefault(x => x.City == model.City);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //https://localhost:{port}/weatherforecast/{city}
        [HttpGet("{city}")]
        [Authorize]
        public ActionResult<WeatherForecast> Get(string city)
        {
            return _sharedMemory.weatherForecasts
                .FirstOrDefault(x => string.Compare(x.City, city, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        // https://localhost:{port}/weatherforecast/{city}
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
