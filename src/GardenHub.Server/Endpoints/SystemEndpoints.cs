using GardenHub.Server.Endpoints.Internal;

namespace GardenHub.Server.Endpoints;

public class SystemEndpoints : IEndpoint
{
    private const string Tag = "System";
    private const string BaseRoute = "system";
    private const string ContentType = "application/json";
    
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, GetSystemInfoAsync).WithName("GetSystemInfo")
            .Produces<string>(200)
            .WithTags(Tag);
    }
    
    internal static async Task<IResult> GetSystemInfoAsync()
    {
        return Results.Ok("The System is up");
    }
}