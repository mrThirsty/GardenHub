using FluentValidation.Results;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Server.DataServices.Services;
using GardenHub.Server.Endpoints.Internal;
using GardenHub.Server.Extensions;
using GardenHub.Shared.Model;
using LanguageExt.Common;

namespace GardenHub.Server.Endpoints;

public class ControllerEndpoints : IEndpoint
{
    private const string Tag = "Sensor Controllers";
    private const string BaseRoute = "controller";
    private const string ContentType = "application/json";
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IControllerDataRepository, ControllerDataRepository>();
        services.AddScoped<IControllerDataService, ControllerDataService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, GetControllersAsync).WithName("GetControllers")
            .Produces<IEnumerable<SensorController>>(200)
            .WithTags(Tag);
        
        app.MapPost(BaseRoute, CreateControllerAsync).WithName("CreateController")
            .Accepts<SensorController>(ContentType)
            .Produces<SensorController>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{id}}", GetControllerAsync).WithName("GetController")
            .Produces<SensorController>(200)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id}}", UpdateControllerAsync).WithName("UpdateController")
            .Accepts<SensorController>(ContentType)
            .Produces<SensorController>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id}}", DeleteControllerAsync).WithName("DeleteController")
            .Produces(204)
            .Produces(404)
            .WithTags(Tag);
    }

    internal static async Task<IResult> GetControllersAsync(IControllerDataService service, ILogger<ControllerEndpoints> logger)
    {
        IEnumerable<SensorController> Sensors = new List<SensorController>();

        try
        {
            Sensors = await service.GetAllAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to get all Controller");

            return Results.Problem(title: "Get Controller", detail: "Unable to get the Controller due to a server error",
                statusCode: 500);
        }

        return Results.Ok(Sensors);
    }
    
    internal static async Task<IResult> CreateControllerAsync(SensorController item, IControllerDataService SensorService, LinkGenerator linker, HttpContext context, ILogger<ControllerEndpoints> logger)
    {
        Result<SensorController> result = null;
        try
        {
            result = await SensorService.CreateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to create Controller");

            return Results.Problem(title: "Create Controller",
                detail: "Unable to create the given Controller due to a server error", statusCode: 500);
        }

        var path = linker.GetUriByName(context, "GetController", new { id = item.Id })!;
        return result.ToCreated(path);
    }
    
    internal static async Task<IResult> GetControllerAsync(Guid id, IControllerDataService service, ILogger<ControllerEndpoints> logger)
    {
        try
        {
            var item = await service.GetByIdAsync(id);

            if(item is not null) return Results.Ok(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get Controller");

            return Results.Problem(title: "Get Controller",
                detail: "Unable to get the given Controller due to a server error", statusCode: 500);
        }
        
        return Results.NotFound();
    }
    
    internal static async Task<IResult> UpdateControllerAsync(Guid id, SensorController item, IControllerDataService service, ILogger<ControllerEndpoints> logger)
    {
        Result<SensorController> result = null;
        try
        {
            item.Id = id;
    
            result = await service.UpdateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update Controller");

            return Results.Problem(title: "Update Controller",
                detail: "Unable to update the given Controller due to a server error", statusCode: 500);
        }

        return result.ToOk();
    }

    internal static async Task<IResult> DeleteControllerAsync(Guid id, IControllerDataService service, ILogger<ControllerEndpoints> logger)
    {
        try
        {
            var deleted = await service.DeleteAsync(id);

            if (!deleted) return Results.NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to delete Controller");

            return Results.Problem(title: "Controller Sensor",
                detail: "Unable to delete the given Controller due to a server error", statusCode: 500);
        }
        
        return Results.NoContent();
    }
}