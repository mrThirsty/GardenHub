using FluentValidation;
using GardenHub.Server.Exceptions;
using LanguageExt.Common;

namespace GardenHub.Server.Extensions;

public static class ResultExtensions
{
    public static IResult ToCreated<tResult>(this Result<tResult> result, string path)
    {
        return result.Match<IResult>(obj =>
        {
            return Results.Created(path, obj);
        }, exception =>
        {
            if (exception is ValidationException validationException)
            {
                return Results.BadRequest(validationException.ToProblemDetails());
            }

            if (exception is OperationException opException)
            {
                return Results.Problem(title: $"{opException.Operation} Error", detail:exception.Message, statusCode: 500);
            }
            
            return Results.StatusCode(500);
        });
    }
    
    public static IResult ToOk<tResult>(this Result<tResult> result)
    {
        return result.Match<IResult>(obj =>
        {
            return Results.Ok(obj);
        }, exception =>
        {
            if (exception is ValidationException validationException)
            {
                return Results.BadRequest(validationException.ToProblemDetails());
            }

            if (exception is OperationException opException)
            {
                return Results.Problem(title: $"{opException.Operation} Error", detail:exception.Message, statusCode: 500);
            }
            
            return Results.StatusCode(500);
        });
    }
}