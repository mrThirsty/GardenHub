using System.Text.Json;
using GardenHub.Monitor.Framework;
using GardenHub.Monitor.Framework.Enums;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Iot.Device.Ads1115;

namespace GardenHub.Monitor.Framework.Config;

public class MonitorConfig
{
    public MonitorConfig()
    {
        // MQTTServer = config.GetValue<string>("MQTT:Server");
        // MQTTPort = config.GetValue<int>("MQTT:Port");
        // MonitorName = config.GetValue<string>("MonitorName");
        // SensorConfig = config.GetSection("Sensors").Get<SensorConfig[]>().ToList();

        MQTTPort = 1883;
        ReportingInterval = new TimeSpan(0, 12, 0, 0, 0);
        Enabled = true;
        MonitorName = $"GardenHubMonitor-{new Random().Next(0,100)}";
        Sensors = new List<SensorConfig>();
        Configured = false;
        ControllerType = ControllerType.FourSensor;
    }
    
    public string MQTTServer { get; set; }
    public int MQTTPort { get; set; }
    public string MonitorName { get; set; }
    public ControllerType ControllerType { get; set; }
    public IList<SensorConfig> Sensors { get; set; }
    
    public TimeSpan ReportingInterval { get; set; }
    public bool Enabled { get; set; }
    
    public bool Configured { get; set; }
    
    private string _configFile = "gardenHub.Monitor.Config.json";
    
    public async Task LoadSetup()
    {
        if (File.Exists(_configFile))
        {
            string content = await File.ReadAllTextAsync(_configFile);

            MonitorConfig details = JsonSerializer.Deserialize<MonitorConfig>(content);

            ReportingInterval = details.ReportingInterval;
            Enabled = details.Enabled;
            Configured = details.Configured;
            Sensors = details.Sensors;
            ControllerType = details.ControllerType;
            MonitorName = details.MonitorName;
            MQTTPort = details.MQTTPort;
            MQTTServer = details.MQTTServer;
        }
        else
        {
            await SaveSetup();
        }
    }

    public async Task SaveSetup()
    {
        string content = JsonSerializer.Serialize(this);
        await File.WriteAllTextAsync(_configFile, content);
    }

    public void SetSensorAddresses()
    {
        foreach (var sensor in Sensors)
        {
            sensor.Address = GetSensorAddress(sensor.Index);
        }
    }

    public void Reconfigure()
    {
        Sensors = new List<SensorConfig>();

        switch (ControllerType)
        {
            case ControllerType.FourSensor:
                CreateSensors(4);
                break;  
            case ControllerType.EightSensor:
                CreateSensors(8);
                break;
            case ControllerType.SixteenSensor:
                CreateSensors(16);
                break;
            default:
                Sensors = new List<SensorConfig>();
                break;
        }
        
        foreach (var sensor in Sensors)
        {
            sensor.SensorName = $"{MonitorName}-{sensor.Index}";
        }
    }
    
    private InputMultiplexer GetSensorAddress(int sensorIndex)
    {
        InputMultiplexer address = InputMultiplexer.AIN0;
        
        switch (sensorIndex)
        {
            case 0:
                address = InputMultiplexer.AIN0;
                break;  
            case 1:
                address = InputMultiplexer.AIN1;
                break;
            case 2:
                address = InputMultiplexer.AIN2;
                break;
            case 3:
                address = InputMultiplexer.AIN3;
                break;
        }

        return address;
    }

    private void CreateSensors(int count)
    {
        for (int index = 0; index < count; index++)
        {
            SensorConfig sensor = new(index, $"{MonitorName}-{index}", GetSensorAddress(index), true);
            Sensors.Add(sensor);
        }
    }
}

