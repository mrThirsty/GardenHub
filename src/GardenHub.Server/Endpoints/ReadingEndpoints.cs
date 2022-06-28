using FluentValidation.Results;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Server.DataServices.Services;
using GardenHub.Server.Endpoints.Internal;
using GardenHub.Server.Extensions;
using GardenHub.Shared.Model;
using LanguageExt.Common;

namespace GardenHub.Server.Endpoints;

public class ReadingEndpoints: IEndpoint
{
    private const string Tag = "Readings";
    private const string BaseRoute = "reading";
    private const string ContentType = "application/json";
    
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IReadingDataRepository, ReadingDataRepository>();
        services.AddScoped<IReadingDataService, ReadingDataService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, GetReadingsAsync).WithName("GetReadings")
            .Produces<IEnumerable<Reading>>(200)
            .WithTags(Tag);
        
        app.MapPost(BaseRoute, CreateReadingAsync).WithName("CreateReading")
            .Accepts<Reading>(ContentType)
            .Produces<Reading>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{id}}", GetReadingAsync).WithName("GetReading")
            .Produces<Reading>(200)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id}}", UpdateReadingAsync).WithName("UpdateReading")
            .Accepts<Reading>(ContentType)
            .Produces<Reading>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id}}", DeleteReadingAsync).WithName("DeleteReading")
            .Produces(204)
            .Produces(404)
            .WithTags(Tag);
    }

    internal static async Task<IResult> GetReadingsAsync(IReadingDataService service, ILogger<ReadingEndpoints> logger)
    {
        IEnumerable<Reading> readings = new List<Reading>();

        try
        {
            readings = await service.GetAllAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to get all readings");

            return Results.Problem(title: "Get Readings", detail: "Unable to get the readings due to a server error",
                statusCode: 500);
        }

        return Results.Ok(readings);
    }
    
    internal static async Task<IResult> CreateReadingAsync(Reading item, IReadingDataService readingService, LinkGenerator linker, HttpContext context, ILogger<ReadingEndpoints> logger)
    {
        Result<Reading> result = null;
        
        try
        {
            result = await readingService.CreateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to create Reading");

            return Results.Problem(title: "Create Reading",
                detail: "Unable to create the given reading due to a server error", statusCode: 500);
        }

        var path = linker.GetUriByName(context, "GetReading", new { id = item.Id })!;
        return result.ToCreated(path);
    }
    
    internal static async Task<IResult> GetReadingAsync(Guid id, IReadingDataService service, ILogger<ReadingEndpoints> logger)
    {
        try
        {
            var item = await service.GetByIdAsync(id);

            if(item is not null) return Results.Ok(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get Reading");

            return Results.Problem(title: "Get Reading",
                detail: "Unable to get the given reading due to a server error", statusCode: 500);
        }
        
        return Results.NotFound();
    }
    
    internal static async Task<IResult> UpdateReadingAsync(Guid id, Reading item, IReadingDataService service, ILogger<ReadingEndpoints> logger)
    {
        Result<Reading> result = null;
        
        try
        {
            item.Id = id;
    
            result = await service.UpdateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update Reading");

            return Results.Problem(title: "Update Reading",
                detail: "Unable to update the given reading due to a server error", statusCode: 500);
        }

        return result.ToOk();
    }

    internal static async Task<IResult> DeleteReadingAsync(Guid id, IReadingDataService service, ILogger<ReadingEndpoints> logger)
    {
        try
        {
            var deleted = await service.DeleteAsync(id);

            if (!deleted) return Results.NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to delete Reading");

            return Results.Problem(title: "delete Reading",
                detail: "Unable to delete the given reading due to a server error", statusCode: 500);
        }
        
        return Results.NoContent();
    }
}