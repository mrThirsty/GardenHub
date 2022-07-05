using System.Text.Json;
using GardenHub.Shared.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using Constants = GardenHub.Shared.Constants;

namespace GardenHub.Monitor.Framework;

public class GardenHubMQTTClient : IGardenHubClient
{
    public GardenHubMQTTClient(MonitorConfig config, Func<Task<IEnumerable<string>>> GetSensorList)
    {
        _factory = new();
        _server = config.MQTTServer;
        _port = config.MQTTPort;
        _clientId = config.MonitorName;

        _GetSensorList = GetSensorList;
    }

    private MqttFactory _factory;
    private IMqttClient _client;
    private bool _connected = false;
    private string _server;
    private int _port;
    private string _clientId;

    private Func<Task<IEnumerable<string>>> _GetSensorList;

    public async Task Connect()
    {
        _client = _factory.CreateMqttClient();
        _client.ApplicationMessageReceivedAsync += ProcessMessage;

        Console.WriteLine($"Connecting to the MQTT broker: {_server}");
        var options = new MqttClientOptionsBuilder().WithClientId(_clientId).WithTcpServer(_server, _port).Build();
        //var options = new MqttClientOptionsBuilder().WithWebSocketServer($"{_url}/mqtt").Build();
        var response = await _client.ConnectAsync(options, CancellationToken.None);

        _connected = response.ResultCode == MqttClientConnectResultCode.Success;

        if (_connected)
        {
            Console.WriteLine("Subscribing to GardenHub message Topic");
            var subscribeDetailOptions = _factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(GardenHub.Shared.Constants.MQTT.Topics.RequestClientDetails)).Build();

            await _client.SubscribeAsync(subscribeDetailOptions, CancellationToken.None);
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
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(args);
        Console.ForegroundColor = ConsoleColor.White;
        if (args.ApplicationMessage.Topic.Equals(Constants.MQTT.Topics.RequestClientDetails))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("GardenHub has requested sensor configuration.");
            Console.ForegroundColor = ConsoleColor.White;
            
            var sensors = await _GetSensorList();

            var payload = new ClientDetailsMessage()
            {
                ControllerId = _clientId,
                Sensors = sensors
            };

            await SendMessage<ClientDetailsMessage>(Constants.MQTT.Topics.ClientDetailsResponse, payload);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("sensor configuration has been sent to GardenHub.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}