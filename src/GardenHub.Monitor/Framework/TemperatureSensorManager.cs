using System.Device.Gpio;
using GardenHub.Monitor.Framework.Config;
using GardenHub.Monitor.Framework.Interfaces;
using GardenHub.Monitor.Framework.Readings;
using Iot.Device.Common;
using Iot.Device.DHTxx;
using UnitsNet;

namespace GardenHub.Monitor.Framework;

public class TemperatureSensorManager : ITemperatureSensorManager
{
    public TemperatureSensorManager(MonitorConfig config)
    {
        _config = config;
    }

    private readonly MonitorConfig _config = default!;
    
    public async Task InitialiseAsync()
    {
        
    }

    public TemperatureReading? GetReadings()
    {
        if (!_config.EnableTemperature) return null;

        using (Dht22 sensor = new(_config.TemperaturePin, PinNumberingScheme.Logical))
        {
            RelativeHumidity humidity = new();
            Temperature temp = new();
            bool successHumidity = sensor.TryReadHumidity(out humidity);
            bool successTemp = sensor.TryReadTemperature(out temp);

            double humidityValue = successHumidity ? humidity.Percent : -100;
            double tempValue = successTemp ? temp.DegreesCelsius : -100;

            double feelsLike = (successHumidity && successTemp)
                ? WeatherHelper.CalculateHeatIndex(temp, humidity).DegreesCelsius
                : -100;
            
            return new(tempValue, successTemp, humidityValue, successHumidity, feelsLike);
        }

        return null;
    }
}