using GardenHub.Server.Data;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Repositories;

public class ReadingDataRepository : DataRepository<Reading>, IReadingDataRepository
{
    public ReadingDataRepository(GardenHubDbContext context) : base(context)
    {
        
    }
}