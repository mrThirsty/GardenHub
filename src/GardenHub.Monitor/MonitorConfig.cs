using Microsoft.Extensions.Configuration;

namespace GardenHub.Monitor;

public class MonitorConfig
{
    public MonitorConfig(IConfiguration config)
    {
        ServerUrl = config.GetValue<string>("ServerUrl");
        MonitorName = config.GetValue<string>("MonitorName");
    }
    
    public string ServerUrl { get; }
    public string MonitorName { get; }
}