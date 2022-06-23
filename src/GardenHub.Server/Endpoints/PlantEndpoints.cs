using FluentValidation.Results;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Server.DataServices.Services;
using GardenHub.Server.Endpoints.Internal;
using GardenHub.Server.Extensions;
using GardenHub.Shared.Model;
using LanguageExt.Common;

namespace GardenHub.Server.Endpoints;

public class PlantEndpoints : IEndpoint
{
    private const string Tag = "Plants";
    private const string BaseRoute = "plant";
    private const string ContentType = "application/json";
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPlantDataRepository, PlantDataRepository>();
        services.AddScoped<IPlantDataService, PlantDataService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, GetPlantsAsync).WithName("GetPlants")
            .Produces<IEnumerable<Plant>>(200)
            .WithTags(Tag);
        
        app.MapPost(BaseRoute, CreatePlantAsync).WithName("CreatePlant")
            .Accepts<Plant>(ContentType)
            .Produces<Plant>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{id}}", GetPlantAsync).WithName("GetPlant")
            .Produces<Plant>(200)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id}}", UpdatePlantAsync).WithName("UpdatePlant")
            .Accepts<Plant>(ContentType)
            .Produces<Plant>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id}}", DeletePlantAsync).WithName("DeletePlant")
            .Produces(204)
            .Produces(404)
            .WithTags(Tag);
    }

    internal static async Task<IResult> GetPlantsAsync(IPlantDataService service, ILogger<PlantEndpoints> logger)
    {
        IEnumerable<Plant> plants = new List<Plant>();

        try
        {
            plants = await service.GetAllAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to get all plants");

            return Results.Problem(title: "Get Plants", detail: "Unable to get the plants due to a server error",
                statusCode: 500);
        }

        return Results.Ok(plants);
    }
    
    internal static async Task<IResult> CreatePlantAsync(Plant plant, IPlantDataService plantService, LinkGenerator linker, HttpContext context, ILogger<PlantEndpoints> logger)
    {
        Result<Plant> result = null;
        try
        {
            result = await plantService.CreateAsync(plant);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to create Plant");

            return Results.Problem(title: "Create Plant",
                detail: "Unable to create the given plant due to a server error", statusCode: 500);
        }

        var path = linker.GetUriByName(context, "GetPlant", new { id = plant.Id })!;
        return result.ToCreated(path);
    }
    
    internal static async Task<IResult> GetPlantAsync(Guid id, IPlantDataService service, ILogger<PlantEndpoints> logger)
    {
        try
        {
            var item = await service.GetByIdAsync(id);

            if(item is not null) return Results.Ok(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get Plant");

            return Results.Problem(title: "Get Plant",
                detail: "Unable to get the given plant due to a server error", statusCode: 500);
        }
        
        return Results.NotFound();
    }
    
    internal static async Task<IResult> UpdatePlantAsync(Guid id, Plant item, IPlantDataService service, ILogger<PlantEndpoints> logger)
    {
        Result<Plant> result = null;
        try
        {
            item.Id = id;
    
            result = await service.UpdateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update Plant");

            return Results.Problem(title: "Update Plant",
                detail: "Unable to update the given plant due to a server error", statusCode: 500);
        }

        return result.ToOk();
    }

    internal static async Task<IResult> DeletePlantAsync(Guid id, IPlantDataService service, ILogger<PlantEndpoints> logger)
    {
        try
        {
            var deleted = await service.DeleteAsync(id);

            if (!deleted) return Results.NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to delete Plant");

            return Results.Problem(title: "delete Plant",
                detail: "Unable to delete the given plant due to a server error", statusCode: 500);
        }
        
        return Results.NoContent();
    }
}