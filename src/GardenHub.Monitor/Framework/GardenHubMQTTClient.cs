using System.Text;
using System.Text.Json;
using GardenHub.Monitor.Framework.Config;
using GardenHub.Monitor.Framework.Events.Payloads;
using GardenHub.Monitor.Framework.Interfaces;
using GardenHub.Shared.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using Constants = GardenHub.Shared.Constants;

namespace GardenHub.Monitor.Framework;

public class GardenHubMQTTClient : IGardenHubClient
{
    public GardenHubMQTTClient(MonitorConfig config, IEventManager events, ILogger<GardenHubMQTTClient> logger)
    {
        _factory = new();
        _server = config.MQTTServer;
        _port = config.MQTTPort;
        _clientId = config.MonitorName;
        _allowConnection = config.EnableMQTT;
        _events = events;
        _logger = logger;
        _sensors = config.Sensors.Where(s => s.Enabled).Select(s => s.SensorName);
    }

    private MqttFactory _factory;
    private IMqttClient _client;

    private bool _allowConnection = false;
    private bool _connected = false;
    private readonly string _server;
    private readonly int _port;
    private readonly string _clientId;
    private readonly IEnumerable<string> _sensors;
    private readonly IEventManager _events;
    private readonly ILogger<GardenHubMQTTClient> _logger;

    public async Task Connect()
    {
        if (_allowConnection)
        {
            _client = _factory.CreateMqttClient();
            _client.ApplicationMessageReceivedAsync += ProcessMessage;

            _logger.LogInformation($"Connecting to the MQTT broker: {_server}");
            var options = new MqttClientOptionsBuilder().WithClientId(_clientId).WithTcpServer(_server, _port).Build();
            var response = await _client.ConnectAsync(options, CancellationToken.None);

            _connected = response.ResultCode == MqttClientConnectResultCode.Success;

            if (_connected)
            {
                _logger.LogInformation("Subscribing to GardenHub message Topic");
                var subscribeDetailOptions = _factory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic(GardenHub.Shared.Constants.MQTT.Topics.RequestClientDetails))
                    .Build();

                await _client.SubscribeAsync(subscribeDetailOptions, CancellationToken.None);

                var subscribeReconfigureOptions = _factory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f =>
                        f.WithTopic($"{GardenHub.Shared.Constants.MQTT.Topics.MonitorReconfigure}/{_clientId}"))
                    .Build();

                await _client.SubscribeAsync(subscribeReconfigureOptions, CancellationToken.None);
            }
        }
    }

    public async Task Disconnect()
    {
        if (_connected)
        {
            var mqttClientDisconnectOptions = _factory.CreateClientDisconnectOptionsBuilder().Build();

            await _client.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
        }
    }

    public async Task SendMessage<T>(string topic, T content)
    {
        if (_connected)
        {
            string jsonPayload = JsonSerializer.Serialize(content);
            var msg = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(jsonPayload).WithContentType("application/json").WithRetainFlag(false).Build();
            
            await _client.PublishAsync(msg, CancellationToken.None);
        }
    }

    public async Task ProcessMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        if (args.ApplicationMessage.Topic.Equals(Constants.MQTT.Topics.RequestClientDetails))
        {
            _logger.LogInformation("GardenHub has requested sensor configuration.");
            
            var payload = new ClientDetailsMessage()
            {
                ControllerId = _clientId,
                Sensors = _sensors
            };

            await SendMessage<ClientDetailsMessage>(Constants.MQTT.Topics.ClientDetailsResponse, payload);
            
            _logger.LogInformation("sensor configuration has been sent to GardenHub.");
        }

        if (args.ApplicationMessage.Topic.Equals($"{Constants.MQTT.Topics.MonitorReconfigure}/{_client}"))
        {
            _logger.LogInformation("GardenHub has sent new configuration.");
            string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            ClientReconfigure config = JsonSerializer.Deserialize<ClientReconfigure>(payload);
            
            // call monitor reconfigure somehow. probable and event of some sort.
            _events.GetEvent<MonitorConfigChangedEvent>().Publish(config);
        }
    }
}