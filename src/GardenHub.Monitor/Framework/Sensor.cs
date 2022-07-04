using Iot.Device.Ads1115;

namespace GardenHub.Monitor.Framework;

public class SensorConfig
{
    public int Index { get; set; }
    public bool Enabled { get; set; }
}

public record Sensor(int Index, bool Enabled, InputMultiplexer Address);
public record SensorReading(int Sensor, double Moisture);