using System.Device.I2c;
using Iot.Device.Ads1115;

namespace GardenHub.Monitor.Framework;

public class SoilMoistureSensorManager
{
    public SoilMoistureSensorManager(int sensorCount = 4)
    {
        _device = I2cDevice.Create(new I2cConnectionSettings(1, 0x48));
        _controller = new(_device);
        _sensorCount = sensorCount;
    }
    
    private readonly I2cDevice _device;// = I2cDevice.Create(new I2cConnectionSettings(1,0x48));
    private readonly Ads1115 _controller; // = new Ads1115(device);
    private readonly int _sensorCount = 0;

    public Dictionary<int, double> GetReadings()
    {
        Dictionary<int, double> values = new Dictionary<int, double>();

        int sensorNumber = 0;

        while (sensorNumber < _sensorCount)
        {
            var reading = _controller.ReadVoltage((InputMultiplexer)sensorNumber);
            values.Add(sensorNumber, reading.Value);
        }

        return values;
    }
}