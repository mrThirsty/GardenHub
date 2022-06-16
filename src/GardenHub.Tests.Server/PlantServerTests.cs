using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GardenHub.Server.Data.Model;
using GardenHub.Tests.Server.TestDataManagers;

namespace GardenHub.Tests.Server;

public class PlantServerTests: IClassFixture<ServerFactory>
{
    public PlantServerTests(ServerFactory factory)
    {
        _factory = factory;
    }

    private readonly ServerFactory _factory;
    private string _baseUrl = "/plant";
    
    [Fact]
    public async Task CreatePlant_CreatesPlant_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = PlantTestDataManager.GeneratePlant();

        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, plant);
        var createdPlant = await result.Content.ReadFromJsonAsync<Plant>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdPlant.Should().BeEquivalentTo(plant, options => options.Excluding(p => p.Id));
        createdPlant.Id.Should().NotBeEmpty();
        result.Headers.Location.Should().Be($"{httpClient.BaseAddress}plant/{createdPlant.Id}");
    }
}