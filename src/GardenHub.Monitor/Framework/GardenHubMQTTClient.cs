using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;

namespace GardenHub.Monitor.Framework;

public class GardenHubMQTTClient : IGardenHubClient
{
    public GardenHubMQTTClient(MonitorConfig config)
    {
        _factory = new();
        _server = config.MQTTServer;
        _port = config.MQTTPort;
        _clientId = config.MonitorName;
    }

    private MqttFactory _factory;
    private IMqttClient _client;
    private bool _connected = false;
    private string _server;
    private int _port;
    private string _clientId;
    
    public async Task Connect()
    {
        _client = _factory.CreateMqttClient();
        
        Console.WriteLine($"Connecting to the MQTT broker: {_server}");
        var options = new MqttClientOptionsBuilder().WithClientId(_clientId).WithTcpServer(_server, _port).Build();
        //var options = new MqttClientOptionsBuilder().WithWebSocketServer($"{_url}/mqtt").Build();
        var response = await _client.ConnectAsync(options, CancellationToken.None);

        _connected = response.ResultCode == MqttClientConnectResultCode.Success;
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
}