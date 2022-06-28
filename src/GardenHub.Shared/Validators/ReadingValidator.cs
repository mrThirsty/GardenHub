using System.Data;
using FluentValidation;
using GardenHub.Shared.Model;

namespace GardenHub.Shared.Validators;

public class ReadingValidator : AbstractValidator<Reading>
{
    public ReadingValidator()
    {
        RuleFor(r => r.PotId).NotEmpty();
        RuleFor(r => r.Timestamp).NotEmpty().GreaterThan(DateTime.MinValue);
    }
}