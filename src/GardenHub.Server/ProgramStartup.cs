using MQTTnet.AspNetCore;

namespace GardenHub.Server;

public static class ProgramStartup
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddHostedMqttServer(server => server.WithoutDefaultEndpoint())
            .AddMqttConnectionHandler()
            .AddConnections();
    }

    public static void ConfigureApplication(IApplicationBuilder app)
    {
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapConnectionHandler<MqttConnectionHandler>("/mqtt",
                options => options.WebSockets.SubProtocolSelector =
                    protocolList => protocolList.FirstOrDefault() ?? string.Empty);
        });

        app.UseMqttServer(server =>
        {
            
        });
    }
}