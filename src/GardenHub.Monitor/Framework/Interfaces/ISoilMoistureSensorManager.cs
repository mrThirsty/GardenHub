using GardenHub.Monitor.Framework.Config;
using GardenHub.Monitor.Framework.Readings;

namespace GardenHub.Monitor.Framework.Interfaces;

public interface ISoilMoistureSensorManager
{
    Task InitialiseAsync();
    //Task<IEnumerable<string>> GetSensors();
    IEnumerable<SensorReading> GetReadings();
}