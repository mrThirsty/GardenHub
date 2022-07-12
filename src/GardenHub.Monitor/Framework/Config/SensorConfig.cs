using Iot.Device.Ads1115;

namespace GardenHub.Monitor.Framework.Config;

public class SensorConfig
{
    public SensorConfig()
    {
        Enabled = true;
    }

    public SensorConfig(int index, string name, InputMultiplexer address, bool enabled = true)
    {
        SensorName = name;
        Index = index;
        Enabled = enabled;
        Address = address;
    }
    
    public int Index { get; set; }
    public string SensorName { get; set; }
    public bool Enabled { get; set; }
    
    public InputMultiplexer Address { get; set; }
}

public record SensorReading(int Sensor, double Moisture);