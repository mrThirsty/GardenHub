namespace GardenHub.Shared.Messages;

public record MoistureReadingMessage(string Controller, string SensorName, double MoistureValue);