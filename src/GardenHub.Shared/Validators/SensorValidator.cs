using FluentValidation;
using GardenHub.Shared.Model;

namespace GardenHub.Shared.Validators;

public class SensorValidator : BaseValidator<Sensor>
{
    public SensorValidator()
    {
        RuleFor(s => s.SensorName).NotEmpty();
    }
}