using GardenHub.Shared.Model;

namespace GardenHub.Web.Services;

public interface IPlantService
{
    Task<IEnumerable<Plant>> GetPlants();
}