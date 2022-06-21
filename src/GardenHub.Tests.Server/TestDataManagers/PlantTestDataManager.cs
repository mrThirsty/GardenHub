using System.Net.Http.Json;
using Bogus;
using GardenHub.Server.Data.Model;

namespace GardenHub.Tests.Server.TestDataManagers;

public class PlantTestDataManager
{
    private const string _plantBaseRoute = "/plant";
    public static Plant GeneratePlant()
    {
        var plantFaker = new Faker<Plant>()
            .RuleFor(p => p.Id, Guid.NewGuid)
            .RuleFor(p => p.PlantName, f => f.Lorem.Text())
            .RuleFor(p => p.RequiredSoilMoisture, f => f.Random.Double(1, 100));

        return plantFaker.Generate();
    }

    public async static Task<Plant> CreatePlant(HttpClient httpClient)
    {
        Plant plant = GeneratePlant();

        var result = await httpClient.PostAsJsonAsync(_plantBaseRoute, plant);
        var createdPlant = await result.Content.ReadFromJsonAsync<Plant>();

        return createdPlant;
    }
}