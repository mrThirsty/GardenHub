using System.Device.I2c;
using Iot.Device.Ads1115;
using System.Linq;

namespace GardenHub.Monitor.Framework;

public class SoilMoistureSensorManager
{
    public SoilMoistureSensorManager(MonitorConfig config)
    {
        _device = I2cDevice.Create(new I2cConnectionSettings(1, 0x48));
        _controller = new(_device);
        _config = config;
    }
    
    private readonly I2cDevice _device;
    private readonly Ads1115 _controller;
    private readonly MonitorConfig _config = default!;
    
    private IList<Sensor> _sensors = default!;
    private double _dry = 2.44875;
    private double _wet = 0.9425;

    public async Task InitialiseAsync()
    {
        _sensors = new List<Sensor>();

        foreach (var sensorConfig in _config.SensorConfig)
        {
            _sensors.Add(new Sensor(sensorConfig.Index, sensorConfig.Enabled,GetSensorAddress(sensorConfig.Index)));
        }
    }
    
    public IEnumerable<SensorReading> GetReadings()
    {
        List<SensorReading> values = new List<SensorReading>();

        var sensorsToRead = _sensors.Where(s => s.Enabled);

        foreach (var sensor in sensorsToRead)
        {
            var reading = _controller.ReadVoltage(sensor.Address);
            // ((_wet-_dry)-(_wet-reading.Value))/(_wet-_dry)
            double percValue = ((_wet - _dry) - (_wet - reading.Value)) / (_wet - _dry);
            
            values.Add(new SensorReading(sensor.Index, percValue));
        }

        return values;
    }

    private InputMultiplexer GetSensorAddress(int sensorIndex)
    {
        InputMultiplexer address = InputMultiplexer.AIN0;
        
        switch (sensorIndex)
        {
            case 0:
                address = InputMultiplexer.AIN0;
                break;  
            case 1:
                address = InputMultiplexer.AIN1;
                break;
            case 2:
                address = InputMultiplexer.AIN2;
                break;
            case 3:
                address = InputMultiplexer.AIN3;
                break;
        }

        return address;
    }
}