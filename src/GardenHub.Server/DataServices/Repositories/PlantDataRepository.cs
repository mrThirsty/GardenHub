using GardenHub.Server.Data;
using GardenHub.Server.Data.Model;
using GardenHub.Server.DataServices.Internal;

namespace GardenHub.Server.DataServices.Repositories;

public class PlantDataRepository : DataRepository<Plant>, IPlantDataRepository
{
    public PlantDataRepository(GardenHubDbContext context) : base(context)
    {
        
    }
}