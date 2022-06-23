using FluentValidation;
using GardenHub.Shared.Model;

namespace GardenHub.Server.Data.Validators;

public class PotValidator: BaseValidator<Pot>
{
    public PotValidator() : base()
    {
        RuleFor(p => p.PotName).NotEmpty();
        RuleFor(p => p.PlantId).NotEmpty();
        RuleFor(p => p.DatePlanted).NotEmpty();
    }
}