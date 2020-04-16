using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using FluentValidation;


namespace api.Validators
{
    public class WeatherForecastValidator : AbstractValidator<WeatherForecast>
    {
        public WeatherForecastValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.City)
                .NotEmpty();
            RuleFor(x => x.details)
                .ForEach(y => y.SetValidator(new DetailValidator()));
        }
    }
}
