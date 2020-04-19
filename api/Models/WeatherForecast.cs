using System;
using System.Collections.Generic;

namespace api.Models
{
    public class WeatherForecast
    {   
        public string City { get; set; }
        public List<Detail> details { get; set; }
    }

    public class Detail
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
