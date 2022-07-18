using GardenHub.Monitor.Framework.Readings;

namespace GardenHub.Monitor.Framework.Interfaces;

public interface ITemperatureSensorManager
{
    Task InitialiseAsync();
    TemperatureReading GetReadings();
}