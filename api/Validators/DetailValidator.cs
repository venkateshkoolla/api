using api.Models;
using FluentValidation;
using System;
using System.Globalization;

namespace api.Validators
{
    public class DetailValidator : AbstractValidator<Detail>
    {
        public DetailValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Date)
                .Must(x => IsDate(x.ToString("yyyy-MM-dd"), "yyyy-MM-dd"))
                .WithMessage("'Date Of weatherforecast' must be a valid date 'yyyy-MM-dd'")
                .NotEmpty();
        }

        public bool IsDate(string source, string format)
        {
            return DateTime.TryParseExact(source, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue);
        }
    }
}
