using System.Collections;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using CommandLine;
using GardenHub.Monitor.Framework;
using GardenHub.Monitor.Framework.Config;
using GardenHub.Monitor.Framework.Enums;
using GardenHub.Monitor.Framework.Events.Payloads;
using GardenHub.Monitor.Framework.Interfaces;
using GardenHub.Shared.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GardenHub.Monitor.Framework.Readings;
using GardenHub.Shared;

namespace GardenHub.Monitor;

public class MonitorManager
{
    public MonitorManager(IServiceProvider services)
    {
        IServiceScope scope = services.CreateScope();
        _services = scope.ServiceProvider;
        _logger = _services.GetRequiredService<ILogger<MonitorManager>>();
        _events = _services.GetRequiredService<IEventManager>();
    }
    
    private readonly IServiceProvider _services;
    private readonly ILogger<MonitorManager> _logger;
    private readonly IEventManager _events;

    private bool _restart = false;
    private bool _runApp = true;
    
    public async void RunOptions(MonitorOptions opts)
    {
        try
        {
            _events.GetEvent<MonitorConfigChangedEvent>().Subscribe(ConfigurationChanged);

            MonitorConfig monitorConfig = _services.GetRequiredService<MonitorConfig>();
            
            if (opts.Configure || !monitorConfig.Configured)
            {
                string capturedValue = "";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Please enter values for the configuration of the monitor, the current settings are displayed in brackets, hit enter to keep current the current value.");
                
                Console.Write($"Monitor Name ({monitorConfig.MonitorName}): ");
                capturedValue = Console.ReadLine();
                monitorConfig.MonitorName =
                    string.IsNullOrWhiteSpace(capturedValue) ? monitorConfig.MonitorName : capturedValue;
                
                Console.Write($"GardenHub MQTT Address ({monitorConfig.MQTTServer}): ");
                capturedValue = Console.ReadLine();
                monitorConfig.MQTTServer =
                    string.IsNullOrWhiteSpace(capturedValue) ? monitorConfig.MQTTServer : capturedValue;
                
                Console.Write($"MQTT Port ({monitorConfig.MQTTPort}): ");
                capturedValue = Console.ReadLine();
                monitorConfig.MQTTPort = string.IsNullOrWhiteSpace(capturedValue) ? monitorConfig.MQTTPort : Convert.ToInt32(capturedValue);
                
                Console.WriteLine("Please enter the Reporting interval in the following format: days:hours:minutes:seconds, eg: 0:1:0:0 is every hour");

                do
                {
                    Console.Write($"Reporting Interval ({monitorConfig.ReportingInterval}): ");
                    capturedValue = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(capturedValue))
                    {
                        TimeSpan parsedValue;
                        bool parsed = TimeSpan.TryParse(capturedValue, out parsedValue);

                        if (!parsed) capturedValue = string.Empty;
                        else
                        {
                            monitorConfig.ReportingInterval = parsedValue;
                        }
                    }
                    else
                    {
                        capturedValue = monitorConfig.ReportingInterval.ToString();
                    }
                } while (string.IsNullOrWhiteSpace(capturedValue));

                Console.WriteLine("Please enter either 4, 8, or 16 for the controller type.");

                do
                {
                    Console.Write($"controller Type ({monitorConfig.ControllerType}): ");
                    capturedValue = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(capturedValue))
                    {
                        ControllerType parsedValue;
                        bool parsed = Enum.TryParse(capturedValue, out parsedValue);

                        if (!parsed) capturedValue = string.Empty;
                        else
                        {
                            monitorConfig.ControllerType = parsedValue;
                        }
                    }
                    else
                    {
                        capturedValue = monitorConfig.ReportingInterval.ToString();
                    }
                } while (string.IsNullOrWhiteSpace(capturedValue));

                do
                {
                    Console.Write($"Enable Temperature sensor ({(monitorConfig.EnableTemperature ? "Yes": "No")}):");
                    capturedValue = Console.ReadLine().ToLower();    
                } while (capturedValue != "yes" && capturedValue != "no" && string.IsNullOrWhiteSpace(capturedValue));

                if(!string.IsNullOrWhiteSpace(capturedValue))
                    monitorConfig.EnableTemperature =  (capturedValue == "yes") ? true : false;

                if (monitorConfig.EnableTemperature)
                {
                    Console.Write($"GPIO Pin for DHT22 sensor ({monitorConfig.TemperaturePin}): ");
                    capturedValue = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(capturedValue))
                        monitorConfig.TemperaturePin = Convert.ToInt32(capturedValue);
                }
                
                monitorConfig.Enabled = true;
                
                monitorConfig.Configured = true;
                monitorConfig.Reconfigure();
                monitorConfig.SetSensorAddresses();
                await monitorConfig.SaveSetup();
            }
            else
            {
                do
                {
                    _restart = false;
                    
                    _logger.LogInformation("Configuring sensors.");
                    ISoilMoistureSensorManager sensorManager = _services.GetRequiredService<ISoilMoistureSensorManager>();//new(monitorConfig);
                    ITemperatureSensorManager tempManager = _services.GetRequiredService<ITemperatureSensorManager>();
                    await sensorManager.InitialiseAsync();
                    await tempManager.InitialiseAsync();

                    _logger.LogInformation("Configuring MQTT...");
                    IGardenHubClient gardenClient = _services.GetRequiredService<IGardenHubClient>();
                    //new GardenHubMQTTClient(monitorConfig, sensorManager.GetSensors);
                    await gardenClient.Connect();

                    while (_runApp)
                    {
                        _logger.LogInformation("Reading sensors");
                        var readings = sensorManager.GetReadings();
                        

                        MonitorReadingMessage msg = new MonitorReadingMessage(monitorConfig.MonitorName);
                        msg.Environment = new()
                        {
                            TemperatureEnabled = monitorConfig.EnableTemperature
                        };

                        if (msg.Environment.TemperatureEnabled)
                        {
                            var tempReading = tempManager.GetReadings();

                            msg.Environment.Humidity = tempReading.Humidty;
                            msg.Environment.Temperature = tempReading.Temperature;
                            msg.Environment.FeelsLike = tempReading.FeelsLike;
                            msg.Environment.ReadingValid = (tempReading.HumidtyValid && tempReading.TemperaturValid);
                        }
                        
                        List<SoilReading> soilReadings = new();

                        foreach (var reading in readings)
                        {
                            soilReadings.Add(new()
                            {
                                Sensor = reading.Sensor,
                                SensorName = $"{monitorConfig.MonitorName}-{reading.Sensor}",
                                MoistureLevel = reading.Moisture
                            });
                        }

                        msg.SoilReadings = soilReadings.ToArray();
                        
                        await gardenClient.SendMessage<MonitorReadingMessage>(Constants.MQTT.Topics.MonitorReading, msg);

                        _logger.LogInformation(JsonSerializer.Serialize(msg));

                        Thread.Sleep(monitorConfig.ReportingInterval);
                    }

                    await gardenClient.Disconnect();
                } while (_restart);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("OOOPS");
            _logger.LogError(ex, "Something went wrong. Try restarting the monitor.");
        }
    }
    
    public void HandleParseError(IEnumerable<Error> errs)
    {
        _logger.LogError("OOOPS");
        _logger.LogError("An error has occurred due to the options selected, review your options and try again");
    }

    private void ConfigurationChanged(ClientReconfigure payload)
    {
        MonitorConfig monitorConfig = _services.GetRequiredService<MonitorConfig>();

        if (payload.ClientId.Equals(monitorConfig.MonitorName))
        {
            monitorConfig.Enabled = payload.Enabled;

            foreach (var sensor in payload.Sensors)
            {
                var configuredSensor = monitorConfig.Sensors.Where(s => s.SensorName.Equals(sensor.SensorId))
                    .FirstOrDefault();

                if (configuredSensor != null)
                {
                    configuredSensor.Enabled = sensor.Enabled;
                }
            }
        }

        monitorConfig.SaveSetup();

        _restart = true;
        _runApp = false;
    }
}