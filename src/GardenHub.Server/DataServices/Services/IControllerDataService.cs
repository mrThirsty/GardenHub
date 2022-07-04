using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Services;

public interface IControllerDataService : IDataService<SensorController>
{
    Task<SensorController?> FindByControllerId(string controllerId);
}