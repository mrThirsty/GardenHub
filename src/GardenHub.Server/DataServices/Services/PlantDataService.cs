using FluentValidation;
using GardenHub.Server.Data.Model;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Server.DataServices.Repositories;

namespace GardenHub.Server.DataServices.Services;

public class PlantDataService : DataService<Plant, IPlantDataRepository>, IPlantDataService
{
    public PlantDataService(IValidator<Plant> validator, IPlantDataRepository repo) : base(validator, repo)
    {
        
    }
}