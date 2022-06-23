using FluentValidation;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Services;

public class PlantDataService : DataService<Plant, IPlantDataRepository>, IPlantDataService
{
    public PlantDataService(IValidator<Plant> validator, IPlantDataRepository repo) : base(validator, repo)
    {
        
    }
}