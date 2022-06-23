using GardenHub.Shared.Model;

namespace GardenHub.Web.Services;

public class PlantService : IPlantService
{
    public PlantService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    private readonly IHttpClientFactory _clientFactory = default!;
    
    public async Task<IEnumerable<Plant>> GetPlants()
    {
        HttpRequestMessage request = new(HttpMethod.Get, "plant");
        var client = _clientFactory.CreateClient("GardenHub");
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var items = await response.Content.ReadFromJsonAsync<IEnumerable<Plant>>();

            return items;
        }

        return new List<Plant>();
    }
}