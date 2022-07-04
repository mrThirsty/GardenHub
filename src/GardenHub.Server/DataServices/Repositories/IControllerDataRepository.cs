using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Repositories;

public interface IControllerDataRepository : IDataRepository<SensorController>
{
    Task<SensorController?> FindByControllerId(string controllerId);
}