namespace GardenHub.Monitor.Framework.Readings;

public record TemperatureReading(double Temperature, bool TemperaturValid, double Humidty, bool HumidtyValid, double FeelsLike);