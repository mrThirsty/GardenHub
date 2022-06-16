using System.Reflection;

namespace GardenHub.Server.Endpoints.Internal;

public static class EndpointExtensions
{
    public static void AddEndpoints(this IServiceCollection services, Type typeMarker, IConfiguration configuration)
    {
        var endpointTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoint.AddServices))!.Invoke(null,
                new object[] { services, configuration });
        }
    }

    

    public static void AddEndpoints<tMarker>(this IServiceCollection services, IConfiguration configuration)
    {
        AddEndpoints(services, typeof(tMarker), configuration);
    }

    public static void UseEndpoints(this IApplicationBuilder app, Type typeMarker)
    {
        var endpointTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoint.DefineEndpoints))!.Invoke(null,
                new object[] { app });
        }
    }
    
    public static void UseEndpoints<tMarker>(this IApplicationBuilder app)
    {
        UseEndpoints(app, typeof(tMarker));   
    }
    
    private static IEnumerable<TypeInfo> GetEndpointTypesFromAssemblyContaining(Type typeMarker)
    {
        var endpointTypes = typeMarker.Assembly.DefinedTypes.Where(x =>
            !x.IsAbstract && !x.IsInterface && typeof(IEndpoint).IsAssignableFrom(x));
        
        return endpointTypes;
    }
}