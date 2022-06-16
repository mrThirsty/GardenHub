namespace GardenHub.Server.Endpoints.Internal;

public interface IEndpoint
{
    public static abstract void AddServices(IServiceCollection services, IConfiguration configuration);
    public static abstract void DefineEndpoints(IEndpointRouteBuilder app);
}