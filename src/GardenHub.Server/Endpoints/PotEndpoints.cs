using FluentValidation.Results;
using GardenHub.Server.Data.Model;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Server.DataServices.Services;
using GardenHub.Server.Endpoints.Internal;
using GardenHub.Server.Extensions;
using LanguageExt.Common;

namespace GardenHub.Server.Endpoints;

public class PotEndpoints : IEndpoint
{
    private const string Tag = "Pots";
    private const string BaseRoute = "pot";
    private const string ContentType = "application/json";
    
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPotDataRepository, PotDataRepository>();
        services.AddScoped<IPotDataService, PotDataService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, GetPlantsAsync).WithName("GetPots")
            .Produces<IEnumerable<Plant>>(200)
            .WithTags(Tag);
        
        app.MapPost(BaseRoute, CreatePlantAsync).WithName("CreatePot")
            .Accepts<Pot>(ContentType)
            .Produces<Pot>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{id}}", GetPlantAsync).WithName("GetPot")
            .Produces<Pot>(200)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id}}", UpdatePlantAsync).WithName("UpdatePot")
            .Accepts<Pot>(ContentType)
            .Produces<Pot>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id}}", DeletePlantAsync).WithName("DeletePot")
            .Produces(204)
            .Produces(404)
            .WithTags(Tag);
    }

    internal static async Task<IResult> GetPlantsAsync(IPotDataService service, ILogger<PotEndpoints> logger)
    {
        IEnumerable<Pot> pots = new List<Pot>();

        try
        {
            pots = await service.GetAllAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to get all pots");

            return Results.Problem(title: "Get Pot", detail: "Unable to get the pots due to a server error",
                statusCode: 500);
        }

        return Results.Ok(pots);
    }
    
    internal static async Task<IResult> CreatePlantAsync(Pot item, IPotDataService plantService, LinkGenerator linker, HttpContext context, ILogger<PotEndpoints> logger)
    {
        Result<Pot> result = null;
        try
        {
            result = await plantService.CreateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to create Pot");

            return Results.Problem(title: "Create Pot",
                detail: "Unable to create the given pot due to a server error", statusCode: 500);
        }

        var path = linker.GetUriByName(context, "GetPot", new { id = item.Id })!;
        return result.ToCreated(path);
    }
    
    internal static async Task<IResult> GetPlantAsync(Guid id, IPotDataService service, ILogger<PotEndpoints> logger)
    {
        try
        {
            var item = await service.GetByIdAsync(id);

            if(item is not null) return Results.Ok(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get Pot");

            return Results.Problem(title: "Get Pot",
                detail: "Unable to get the given pot due to a server error", statusCode: 500);
        }
        
        return Results.NotFound();
    }
    
    internal static async Task<IResult> UpdatePlantAsync(Guid id, Pot item, IPotDataService service, ILogger<PotEndpoints> logger)
    {
        Result<Pot> result = null;
        try
        {
            item.Id = id;
    
            result = await service.UpdateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update Pot");

            return Results.Problem(title: "Update Pot",
                detail: "Unable to update the given pot due to a server error", statusCode: 500);
        }

        return result.ToOk();
    }

    internal static async Task<IResult> DeletePlantAsync(Guid id, IPotDataService service, ILogger<PotEndpoints> logger)
    {
        try
        {
            var deleted = await service.DeleteAsync(id);

            if (!deleted) return Results.NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to delete Pot");

            return Results.Problem(title: "delete Pot",
                detail: "Unable to delete the given pot due to a server error", statusCode: 500);
        }
        
        return Results.NoContent();
    }
}