namespace GardenHub.Shared.Messages;

public class ClientReconfigure
{
    public string ClientId { get; set; }
    public bool Enabled { get; set; }
    public IEnumerable<SensorReconfigure> Sensors { get; set; } 

}

public class SensorReconfigure
{
    public string SensorId { get; set; }
    public bool Enabled { get; set; }
}