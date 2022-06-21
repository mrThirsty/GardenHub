using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace GardenHub.Server.Exceptions;

public static class ExceptionExtensions
{
    public static ValidationProblemDetails ToProblemDetails(
        this ValidationException ex)
    {
        var error = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = 400
        };

        if (ex.Errors.Count() == 0) error.Detail = ex.Message;
        
        foreach (var validationFailure in ex.Errors)
        {
            if (error.Errors.ContainsKey(validationFailure.PropertyName))
            {
                error.Errors[validationFailure.PropertyName] =
                    error.Errors[validationFailure.PropertyName]
                        .Concat(new[] { validationFailure.ErrorMessage }).ToArray(); 
                continue;
            }
                
            error.Errors.Add(new KeyValuePair<string, string[]>(
                validationFailure.PropertyName, 
                new[] { validationFailure.ErrorMessage }));
        }

        return error;
    }
}