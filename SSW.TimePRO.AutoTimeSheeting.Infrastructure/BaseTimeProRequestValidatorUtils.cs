using FluentValidation;
using System;
using System.Globalization;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure
{
    public static class BaseTimeProRequestValidatorUtils
    {
        public static void ForEmpID<T>(this IRuleBuilderInitial<T, string> initialRule)
        {
            initialRule
                .NotEmpty()
                .WithMessage("Employee ID must not be empty");
        }

        public static void ForTenantUrl<T>(this IRuleBuilderInitial<T, string> initialRule)
            where T : BaseTimeProRequest 
        {
            initialRule
                .NotEmpty()
                .WithMessage("Tenant URL must not be empty")
                .Must(arg => Uri.TryCreate(arg, UriKind.Absolute, out var _))
                .WithMessage("Tenant URL must be a valid URL");
        }

        public static void ForToken<T>(this IRuleBuilderInitial<T, string> initialRule)
        {
            initialRule
                .NotEmpty()
                .WithMessage("Token must not be empty");
        }

        public static void ForUsername<T>(this IRuleBuilderInitial<T, string> initialRule)
        {
            initialRule
                .NotEmpty()
                .WithMessage("Username must not be empty");
        }

        public static void ForDate<T>(this IRuleBuilderInitial<T, string> initialRule, string name)
        {
            initialRule
                .NotEmpty()
                .WithMessage($"{name} must not be empty")
                .Must(rawDate =>
                {
                    var isValid = DateTime.TryParse(rawDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date);
                    return isValid && date != DateTime.MinValue;
                })
                .WithMessage($"{name} is not in validate format");
        }
    }
}
