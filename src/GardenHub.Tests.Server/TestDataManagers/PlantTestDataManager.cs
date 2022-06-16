using Bogus;
using GardenHub.Server.Data.Model;

namespace GardenHub.Tests.Server.TestDataManagers;

public class PlantTestDataManager
{
    public static Plant GeneratePlant()
    {
        var plantFaker = new Faker<Plant>()
            .RuleFor(p => p.Id, Guid.NewGuid)
            .RuleFor(p => p.PlantName, f => f.Lorem.Text());

        return plantFaker.Generate();
    }
}