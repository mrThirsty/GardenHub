namespace GardenHub.Monitor.Framework;

public interface IGardenHubClient
{
    Task Connect();
    Task Disconnect();
    Task SendMessage<T>(string topic, T content);
}