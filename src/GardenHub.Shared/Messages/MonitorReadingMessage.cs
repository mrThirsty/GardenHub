namespace GardenHub.Shared.Messages;

public class MonitorReadingMessage
{
    public MonitorReadingMessage(string name)
    {
        Monitor = name;
    }
    
    public string Monitor { get; }

    public EnvironmentReading Environment { get; set; }
    
    public SoilReading[] SoilReadings { get; set; }
}

public class EnvironmentReading
{
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public double Humidity { get; set; }
    public bool TemperatureEnabled { get; set; }
    public bool ReadingValid { get; set; }
}
public class SoilReading
{
    public int Sensor { get; set; }
    public string SensorName { get; set; }
    public double MoistureLevel { get; set; }
}