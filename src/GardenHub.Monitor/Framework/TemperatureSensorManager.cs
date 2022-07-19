using System.Device.Gpio;
using GardenHub.Monitor.Framework.Config;
using GardenHub.Monitor.Framework.Interfaces;
using GardenHub.Monitor.Framework.Readings;
using Iot.Device.Common;
using Iot.Device.DHTxx;
using Microsoft.Extensions.Logging;
using UnitsNet;

namespace GardenHub.Monitor.Framework;

public class TemperatureSensorManager : ITemperatureSensorManager
{
    public TemperatureSensorManager(MonitorConfig config, ILogger<TemperatureSensorManager> logger)
    {
        _config = config;
        _logger = logger;
    }

    private readonly MonitorConfig _config = default!;
    private readonly ILogger<TemperatureSensorManager> _logger;
    
    public async Task InitialiseAsync()
    {
        
    }

    public TemperatureReading? GetReadings()
    {
        if (!_config.EnableTemperature)
        {
            _logger.LogInformation("Temperature is not enabled, will not read the  temperature and humidity");
            return null;
        }

        try
        {
            _logger.LogInformation($"Setting up the DHT22 sensor on pin {_config.TemperaturePin}.");
            
            using (Dht22 sensor = new(_config.TemperaturePin))
            {
                RelativeHumidity humidity = default;
                Temperature temp = default;
                
                _logger.LogInformation("Trying to read the environment details from sensor");
                bool success = sensor.TryReadHumidity(out humidity) && sensor.TryReadTemperature(out temp);
                
                //bool successHumidity = sensor.TryReadHumidity(out humidity);
                //bool successTemp = sensor.TryReadTemperature(out temp);

                _logger.LogInformation(($"Able to read Temperature & Humidity: {success}"));
                //_logger.LogInformation(($"Able to read Temperature: {successTemp}"));
                _logger.LogInformation("trying to getting values");
                
                double humidityValue = success ? humidity.Percent : -100;
                double tempValue = success ? temp.DegreesCelsius : -100;

                double feelsLike = success
                    ? WeatherHelper.CalculateHeatIndex(temp, humidity).DegreesCelsius
                    : -100;
            
                _logger.LogInformation($"Temperature: {tempValue}");
                _logger.LogInformation($"Humidity: {humidityValue}");
                _logger.LogInformation($"Feels Like: {feelsLike}");
                
                return new(tempValue, success, humidityValue, success, feelsLike);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't use the temp sensor");
        }
        
        return null;
    }
}