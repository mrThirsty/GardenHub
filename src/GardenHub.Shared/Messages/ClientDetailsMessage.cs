namespace GardenHub.Shared.Messages;

public class ClientDetailsMessage
{
    public string ControllerId { get; set; }
    public IEnumerable<string> Sensors { get; set; }
}