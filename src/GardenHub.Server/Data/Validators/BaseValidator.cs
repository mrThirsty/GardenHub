using FluentValidation;
using GardenHub.Server.Data.Internal;

namespace GardenHub.Server.Data.Validators;

public class BaseValidator<T> : AbstractValidator<T> where T : EntityBase
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}