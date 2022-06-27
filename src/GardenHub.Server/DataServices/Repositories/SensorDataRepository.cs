using GardenHub.Server.Data;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Repositories;

public class SensorDataRepository : DataRepository<Sensor>, ISensorDataRepository
{
    public SensorDataRepository(GardenHubDbContext context) : base(context)
    {
    }
}