using FluentValidation;
using GardenHub.Shared.Model;

namespace GardenHub.Shared.Validators;

public class ControllerValidator: BaseValidator<SensorController>
{
    public ControllerValidator() : base()
    {
        RuleFor(c => c.ControllerId).NotEmpty();
        RuleFor(c => c.Enabled).NotEmpty();
        RuleFor(c => c.ReportingInterval).NotEmpty();
    }
}