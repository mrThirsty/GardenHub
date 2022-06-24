using FluentValidation;
using GardenHub.Shared.Model;

namespace GardenHub.Shared.Validators;

public class PlantValidator : BaseValidator<Plant>
{
    public PlantValidator() : base()
    {
        RuleFor(p => p.PlantName).NotEmpty();
        RuleFor(p => p.RequiredSoilMoisture).GreaterThan(0);
    }
}