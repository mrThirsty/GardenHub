using System.Text;
using Ardalis.GuardClauses;
using GardenHub.Server.DataServices.Services;
using MQTTnet.AspNetCore;
using MQTTnet.Server;

namespace GardenHub.Server.MQTT;

public class MqttService
{
    public MqttService(IControllerDataService controllerService, ISensorDataService sensorService)
    {
        Guard.Against.Null(controllerService, nameof(controllerService));
        Guard.Against.Null(sensorService, nameof(sensorService));
        
        _controllerService = controllerService;
        _sensorService = sensorService;
    }

    private readonly IControllerDataService _controllerService;
    private readonly ISensorDataService _sensorService;
    private MqttServer _mqttServer;
    
    public void ConfigureMqttServerOptions(AspNetMqttServerOptionsBuilder options)
    {
        options.WithoutDefaultEndpoint();
    }

    public void ConfigureMqttServer(MqttServer mqtt)
    {
        _mqttServer = mqtt;
        _mqttServer.ClientConnectedAsync += async (args) =>
        {
            Console.WriteLine($"MQTT: Client connected: {args.ClientId}");

            var controller = await _controllerService.FindByControllerId(args.ClientId);

            if (controller is null)
            {
                controller = new()
                {
                    ControllerId = args.ClientId
                };

                await _controllerService.CreateAsync(controller);
            }
        };
        
        _mqttServer.ClientDisconnectedAsync += async (args) =>
        {
            Console.WriteLine($"MQTT: Client disconnected: {args.ClientId}");
        };
        
        _mqttServer.InterceptingPublishAsync += async (args) =>
        {
            var msg = args.ApplicationMessage;
        
            var topic = msg.Topic;
            var payload = Encoding.UTF8.GetString(msg.Payload);
            
            Console.WriteLine($"New MQTT message: {topic}");
            Console.WriteLine($"Payload: {payload}");
        };
    }
}