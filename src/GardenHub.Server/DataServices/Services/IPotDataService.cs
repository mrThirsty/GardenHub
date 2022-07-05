using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Services;

public interface IPotDataService : IDataService<Pot>
{
    Task<Pot?> FindPotBySensorId(Guid sensorId);
}