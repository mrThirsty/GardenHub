using GardenHub.Shared.Model;
using GardenHub.Web.Services.Internal;

namespace GardenHub.Web.Services.Data;

public class SensorService : BaseDataService<SensorService, Sensor>, ISensorService
{
    public SensorService(IHttpClientFactory clientFactory, IMessageHandler msgHandler, ILogger<SensorService> logger) : base("sensor","SensorService",clientFactory, msgHandler, logger)
    {
    }
}