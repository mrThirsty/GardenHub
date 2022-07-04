using GardenHub.Monitor.Framework;
using Microsoft.Extensions.Configuration;

namespace GardenHub.Monitor;

public class MonitorConfig
{
    public MonitorConfig(IConfiguration config)
    {
        MQTTServer = config.GetValue<string>("MQTT:Server");
        MQTTPort = config.GetValue<int>("MQTT:Port");
        MonitorName = config.GetValue<string>("MonitorName");
        SensorConfig = config.GetSection("Sensors").Get<SensorConfig[]>().ToList();
        ReadingDelay = config.GetValue<int>("ReadingDelay");
    }
    
    public string MQTTServer { get; }
    public int MQTTPort { get; }
    public string MonitorName { get; }
    public IEnumerable<SensorConfig> SensorConfig { get; }
    
    public int ReadingDelay { get; }
}

