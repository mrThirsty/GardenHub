using GardenHub.Server.Data;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Repositories;

public class PlantDataRepository : DataRepository<Plant>, IPlantDataRepository
{
    public PlantDataRepository(GardenHubDbContext context) : base(context)
    {
        
    }
}