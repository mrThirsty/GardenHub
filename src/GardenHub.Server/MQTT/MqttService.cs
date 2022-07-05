using System.Text;
using System.Text.Json;
using Ardalis.GuardClauses;
using GardenHub.Server.DataServices.Services;
using GardenHub.Shared;
using GardenHub.Shared.Messages;
using GardenHub.Shared.Model;
using Hangfire.Logging;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Server;

namespace GardenHub.Server.MQTT;

public class MqttService
{
    public MqttService(IServiceScopeFactory serviceScopeFactory, ILogger<MqttService> logger)//IControllerDataService controllerService, ISensorDataService sensorService)
    {
        // Guard.Against.Null(controllerService, nameof(controllerService));
        // Guard.Against.Null(sensorService, nameof(sensorService));
        Guard.Against.Null(serviceScopeFactory, nameof(serviceScopeFactory));
        Guard.Against.Null(logger, nameof(logger));
        
        _servicesScopeFactory = serviceScopeFactory;
        
        // _controllerService = controllerService;
        // _sensorService = sensorService;
    }

    // private readonly IControllerDataService _controllerService;
    // private readonly ISensorDataService _sensorService;
    private readonly IServiceScopeFactory _servicesScopeFactory;
    private readonly ILogger<MqttService> _logger;
    
    private MqttServer _mqttServer;
    
    public void ConfigureMqttServerOptions(AspNetMqttServerOptionsBuilder options)
    {
        options.WithoutDefaultEndpoint();
    }

    public void ConfigureMqttServer(MqttServer mqtt)
    {
        _mqttServer = mqtt;
        _mqttServer.ClientConnectedAsync += ClientConnectedAsync;

        _mqttServer.ClientDisconnectedAsync += ClientDisconnectedAsync;

        _mqttServer.InterceptingPublishAsync += InterceptMessageAsync;
        
        _mqttServer.ClientSubscribedTopicAsync += ClientSubscribedTopicAsync;
    }

    private async Task ClientConnectedAsync(ClientConnectedEventArgs args)
    {
        try
        {
            Console.WriteLine($"MQTT: Client connected: {args.ClientId}");

            using (var servicesScope = _servicesScopeFactory.CreateScope())
            {
                var controllerService = servicesScope.ServiceProvider.GetService<IControllerDataService>();
                
                var controller = await controllerService.FindByControllerId(args.ClientId);

                if (controller is null)
                {
                    controller = new()
                    {
                        ControllerId = args.ClientId
                    };

                    await controllerService.CreateAsync(controller);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in MqttService.ClientConnectedAsync");
        }
    }

    private async Task ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs args)
    {
        if (args.TopicFilter.Topic.Equals(Constants.MQTT.Topics.RequestClientDetails))
        {
            ClientDetailsRequestMessage msg = new()
            {
                ControllerId = args.ClientId
            };

            var appMsg = new MqttApplicationMessageBuilder()
                .WithTopic(Constants.MQTT.Topics.RequestClientDetails)
                .WithPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg)))
                .Build();
                    
            InjectedMqttApplicationMessage payload = new(appMsg)
            {
                SenderClientId = "GardenHubBroker"
            };

            await _mqttServer.InjectApplicationMessage(payload);
        }
    }
    
    private async Task ClientDisconnectedAsync(ClientDisconnectedEventArgs args)
    {
        try
        {
            Console.WriteLine($"MQTT: Client disconnected: {args.ClientId}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in MqttService.ClientDisconnectedAsync");
        }
    }

    private async Task InterceptMessageAsync(InterceptingPublishEventArgs args)
    {
        try
        {
            var topic = args.ApplicationMessage.Topic;
            
            if(!string.IsNullOrWhiteSpace(topic))
            {
                var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            
                Console.WriteLine($"New MQTT message: {topic}");
                Console.WriteLine($"Payload: {payload}");

                if (topic!.Equals(Constants.MQTT.Topics.SoilMoistureReading, StringComparison.InvariantCultureIgnoreCase))
                {
                    MoistureReadingMessage? msg = JsonSerializer.Deserialize<MoistureReadingMessage>(payload);

                    if(msg is not null) await HandleSoilMoistureReading(msg!);
                }
                
                if (topic!.Equals(Constants.MQTT.Topics.ClientDetailsResponse, StringComparison.InvariantCultureIgnoreCase))
                {
                    ClientDetailsMessage? msg = JsonSerializer.Deserialize<ClientDetailsMessage>(payload);

                    if(msg is not null) await HandleClientDetails(msg!);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in MqttService.InterceptingPublishAsync");
        }
    }

    private async Task HandleClientDetails(ClientDetailsMessage msg)
    {
        using (var scope = _servicesScopeFactory.CreateScope())
        {
            IControllerDataService controllerService =
                scope.ServiceProvider.GetRequiredService<IControllerDataService>();
            ISensorDataService sensorService = scope.ServiceProvider.GetRequiredService<ISensorDataService>();

            var controller = await controllerService.FindByControllerId(msg.ControllerId);

            if (controller is not null)
            {
                foreach (string sensorName in msg.Sensors)
                {
                    var sensor = await sensorService.GetSensorByName(sensorName);

                    if (sensor is null)
                    {
                        sensor = new()
                        {
                            SensorName = sensorName,
                            SensorControllerId = controller!.Id
                        };

                        await sensorService.CreateAsync(sensor);
                    }
                }
            }
        }
    }

    private async Task HandleSoilMoistureReading(MoistureReadingMessage msg)
    {
        using (var scope = _servicesScopeFactory.CreateScope())
        {
            IReadingDataService readingService = scope.ServiceProvider.GetRequiredService<IReadingDataService>();
            ISensorDataService sensorService = scope.ServiceProvider.GetRequiredService<ISensorDataService>();
            IPotDataService potService = scope.ServiceProvider.GetRequiredService<IPotDataService>();

            var sensor = await sensorService.GetSensorByName(msg.SensorName);

            if (sensor is not null)
            {
                var currentPot = await potService.FindPotBySensorId(sensor!.Id);

                if (currentPot is not null)
                {
                    Reading newReading = new()
                    {
                        PotId = currentPot!.Id,
                        Timestamp = DateTime.UtcNow,
                        SoilMoistureReading = msg.MoistureValue
                    };

                    await readingService.CreateAsync(newReading);
                }
            }
        }
    }
}