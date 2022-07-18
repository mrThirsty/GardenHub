namespace GardenHub.Shared;

public static class Constants
{
    public static class MQTT
    {
        public static class Topics
        {
            public const string MonitorReading = "MonitorReading";
            public const string RequestClientDetails = "RequestClientDetails";
            public const string ClientDetailsResponse = "ClientDetailsResponse";
            public const string MonitorReconfigure = "MonitorReconfigure";
        }
    }
}