using GardenHub.Shared.Model;
using GardenHub.Web.Services.Internal;

namespace GardenHub.Web.Services.Data;

public class PotService : BaseDataService<PotService, Pot>, IPotService
{
    public PotService(IHttpClientFactory clientFactory, IMessageHandler msgHandler, ILogger<PotService> logger) : base("pot","PotService",clientFactory, msgHandler, logger)
    {
    }
}