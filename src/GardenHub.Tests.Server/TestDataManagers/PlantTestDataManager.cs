using System.Net.Http.Json;
using Bogus;
using GardenHub.Shared.Model;

namespace GardenHub.Tests.Server.TestDataManagers;

public class PlantTestDataManager
{
    private const string _plantBaseRoute = "/plant";
    public static Plant GenerateItem()
    {
        var plantFaker = new Faker<Plant>()
            .RuleFor(p => p.Id, Guid.NewGuid)
            .RuleFor(p => p.PlantName, f => f.Lorem.Text())
            .RuleFor(p => p.RequiredSoilMoisture, f => f.Random.Double(1, 100))
            .RuleFor(p => p.RequiredSun, f => f.PickRandom<LightLevel>());

        return plantFaker.Generate();
    }

    public async static Task<Plant> CreateItem(HttpClient httpClient)
    {
        Plant plant = GenerateItem();

        var result = await httpClient.PostAsJsonAsync(_plantBaseRoute, plant);
        var createdPlant = await result.Content.ReadFromJsonAsync<Plant>();

        return createdPlant;
    }

    public async static Task DeleteItems(IEnumerable<Guid> idsToDelete, HttpClient client)
    {
        foreach (var id in idsToDelete)
        {
            await DeleteItem(id, client);
        }
    }

    public async static Task DeleteItem(Guid id, HttpClient client)
    {
        await client.DeleteAsync($"{_plantBaseRoute}/{id}");
    }
}