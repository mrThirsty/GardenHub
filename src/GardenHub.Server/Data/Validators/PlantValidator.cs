using FluentValidation;
using GardenHub.Server.Data.Model;

namespace GardenHub.Server.Data.Validators;

public class PlantValidator : BaseValidator<Plant>
{
    public PlantValidator() : base()
    {
        RuleFor(p => p.PlantName).NotEmpty();
        RuleFor(p => p.RequiredSoilMoisture).GreaterThan(0);
    }
}