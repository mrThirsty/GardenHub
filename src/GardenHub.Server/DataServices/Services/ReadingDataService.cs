using FluentValidation;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Services;

public class ReadingDataService : DataService<Reading, IReadingDataRepository>, IReadingDataService
{
    public ReadingDataService(IValidator<Reading> validator, IReadingDataRepository repo) : base(validator, repo)
    {
        
    }
}