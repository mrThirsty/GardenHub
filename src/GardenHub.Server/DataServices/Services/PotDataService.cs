using FluentValidation;
using GardenHub.Server.Data.Model;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Server.DataServices.Repositories;

namespace GardenHub.Server.DataServices.Services;

public class PotDataService : DataService<Pot, IPotDataRepository>, IPotDataService
{
    public PotDataService(IValidator<Pot> validator, IPotDataRepository repo) : base(validator, repo)
    {
        
    }
}