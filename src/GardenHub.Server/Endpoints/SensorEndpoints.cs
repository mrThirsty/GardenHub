using FluentValidation.Results;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Server.DataServices.Services;
using GardenHub.Server.Endpoints.Internal;
using GardenHub.Server.Extensions;
using GardenHub.Shared.Model;
using LanguageExt.Common;

namespace GardenHub.Server.Endpoints;

public class SensorEndpoints : IEndpoint
{
    private const string Tag = "Sensors";
    private const string BaseRoute = "sensor";
    private const string ContentType = "application/json";
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISensorDataRepository, SensorDataRepository>();
        services.AddScoped<ISensorDataService, SensorDataService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, GetSensorsAsync).WithName("GetSensors")
            .Produces<IEnumerable<Sensor>>(200)
            .WithTags(Tag);
        
        app.MapPost(BaseRoute, CreateSensorAsync).WithName("CreateSensor")
            .Accepts<Sensor>(ContentType)
            .Produces<Sensor>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);
        
        app.MapGet($"{BaseRoute}/{{id}}", GetSensorAsync).WithName("GetSensor")
            .Produces<Sensor>(200)
            .Produces(404)
            .WithTags(Tag);
        
        app.MapPut($"{BaseRoute}/{{id}}", UpdateSensorAsync).WithName("UpdateSensor")
            .Accepts<Sensor>(ContentType)
            .Produces<Sensor>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id}}", DeleteSensorAsync).WithName("DeleteSensor")
            .Produces(204)
            .Produces(404)
            .WithTags(Tag);
    }

    internal static async Task<IResult> GetSensorsAsync(ISensorDataService service, ILogger<SensorEndpoints> logger)
    {
        IEnumerable<Sensor> Sensors = new List<Sensor>();

        try
        {
            Sensors = await service.GetAllAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to get all Sensors");

            return Results.Problem(title: "Get Sensors", detail: "Unable to get the Sensors due to a server error",
                statusCode: 500);
        }

        return Results.Ok(Sensors);
    }
    
    internal static async Task<IResult> CreateSensorAsync(Sensor item, ISensorDataService SensorService, LinkGenerator linker, HttpContext context, ILogger<SensorEndpoints> logger)
    {
        Result<Sensor> result = null;
        try
        {
            result = await SensorService.CreateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to create Sensor");

            return Results.Problem(title: "Create Sensor",
                detail: "Unable to create the given Sensor due to a server error", statusCode: 500);
        }

        var path = linker.GetUriByName(context, "GetSensor", new { id = item.Id })!;
        return result.ToCreated(path);
    }
    
    internal static async Task<IResult> GetSensorAsync(Guid id, ISensorDataService service, ILogger<SensorEndpoints> logger)
    {
        try
        {
            var item = await service.GetByIdAsync(id);

            if(item is not null) return Results.Ok(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get Sensor");

            return Results.Problem(title: "Get Sensor",
                detail: "Unable to get the given Sensor due to a server error", statusCode: 500);
        }
        
        return Results.NotFound();
    }
    
    internal static async Task<IResult> UpdateSensorAsync(Guid id, Sensor item, ISensorDataService service, ILogger<SensorEndpoints> logger)
    {
        Result<Sensor> result = null;
        try
        {
            item.Id = id;
    
            result = await service.UpdateAsync(item);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update Sensor");

            return Results.Problem(title: "Update Sensor",
                detail: "Unable to update the given Sensor due to a server error", statusCode: 500);
        }

        return result.ToOk();
    }

    internal static async Task<IResult> DeleteSensorAsync(Guid id, ISensorDataService service, ILogger<SensorEndpoints> logger)
    {
        try
        {
            var deleted = await service.DeleteAsync(id);

            if (!deleted) return Results.NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to delete Sensor");

            return Results.Problem(title: "delete Sensor",
                detail: "Unable to delete the given Sensor due to a server error", statusCode: 500);
        }
        
        return Results.NoContent();
    }
}