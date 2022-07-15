using GardenHub.Shared.Model;
using GardenHub.Web.Services.Internal;

namespace GardenHub.Web.Services.Data;

public class ControllerService :  BaseDataService<ControllerService, SensorController>, IControllerService
{
    public ControllerService(IHttpClientFactory clientFactory, IMessageHandler msgHandler, ILogger<ControllerService> logger) : base("controller","ControllerService",clientFactory, msgHandler, logger)
    {
    }
}