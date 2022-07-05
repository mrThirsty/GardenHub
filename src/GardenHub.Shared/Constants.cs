namespace GardenHub.Shared;

public static class Constants
{
    public static class MQTT
    {
        public static class Topics
        {
            public const string SoilMoistureReading = "SoilMoistureReading";
            public const string RequestClientDetails = "RequestClientDetails";
            public const string ClientDetailsResponse = "ClientDetailsResponse";
        }
    }
}