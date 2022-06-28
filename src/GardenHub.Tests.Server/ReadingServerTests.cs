using System.Net;
using System.Net.Http.Json;
using GardenHub.Shared.Model;
using GardenHub.Tests.Server.TestDataManagers;
using Microsoft.AspNetCore.Mvc;

namespace GardenHub.Tests.Server;

public class ReadingServerTests : IClassFixture<ServerFactory>, IAsyncLifetime
{
    public ReadingServerTests(ServerFactory factory)
    {
        _factory = factory;
    }

    private readonly ServerFactory _factory;
    private string _baseUrl = "/reading";
    private List<Guid> _IdsToDelete = new();
    private List<Guid> _plantsToDelete = new();
    public List<Guid> _potsToDelete = new();

    [Fact]
    public async Task GetReading_ReturnsReading_WhenReadingExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await ReadingTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _potsToDelete.Add(item.PotId);
        _plantsToDelete.Add(item.Pot.PlantId);
        
        //Act
        var result = await httpClient.GetAsync($"{_baseUrl}/{item.Id}");
        var foundItem = await result.Content.ReadFromJsonAsync<Reading>();

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        foundItem.Should().BeEquivalentTo(item, options => options.Excluding((r => r.Pot)));
    }
    
    [Fact]
    public async Task GetReading_ReturnsNotFound_WhenReadingDoesNotExist()
    {
        //Arrange
        var httpClient = _factory.CreateClient();

        //Act
        var result = await httpClient.GetAsync($"{_baseUrl}/{Guid.NewGuid()}");

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetAllReadings_ReturnsAllReadings_WhenReadingsExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await ReadingTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _potsToDelete.Add(item.PotId);
        _plantsToDelete.Add(item.Pot.PlantId);

        //Act
        var result = await httpClient.GetAsync(_baseUrl);
        var foundItems = await result.Content.ReadFromJsonAsync<List<Reading>>();

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        foundItems.Count.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task CreateReading_CreatesReading_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var pot = await PotTestDataManager.CreateItem(httpClient);
        _potsToDelete.Add(pot.Id);
        _plantsToDelete.Add(pot.PlantId);
        
        var item = ReadingTestDataManager.GenerateItem(pot.Id);
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var createdItem = await result.Content.ReadFromJsonAsync<Reading>();
        _IdsToDelete.Add(createdItem.Id);
        
        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdItem.Should().BeEquivalentTo(item, options => options.Excluding(p => p.Id));
        createdItem.Id.Should().NotBeEmpty();
        result.Headers.Location.Should().Be($"{httpClient.BaseAddress}reading/{createdItem.Id}");
    }
    
    [Fact]
    public async Task CreateReading_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var pot = await PotTestDataManager.CreateItem(httpClient);
        _potsToDelete.Add(pot.Id);
        _plantsToDelete.Add(pot.PlantId);
        
        var item = ReadingTestDataManager.GenerateItem(pot.Id);
        item.Timestamp = DateTime.MinValue;
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("Timestamp");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Timestamp' must not be empty.");
    }
    
    [Fact]
    public async Task CreateReading_Fails_WhenReadingExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await ReadingTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _potsToDelete.Add(item.PotId);
        _plantsToDelete.Add(item.Pot.PlantId);
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors!.Detail.Should().Be("Record already exists");
    }

    [Fact]
    public async Task UpdateReading_UpdatesReading_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await ReadingTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _potsToDelete.Add(item.PotId);
        _plantsToDelete.Add(item.Pot.PlantId);
        
        item.Timestamp = DateTime.Now;
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item);
        var updatedReading = await result.Content.ReadFromJsonAsync<Reading>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedReading.Should().BeEquivalentTo(item);
    }
    
    [Fact]
    public async Task UpdateReading_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await ReadingTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _potsToDelete.Add(item.PotId);
        _plantsToDelete.Add(item.Pot.PlantId);
        
        item.Timestamp = DateTime.MinValue;
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("Timestamp");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Timestamp' must not be empty.");
    }

    [Fact]
    public async Task UpdateReading_Fails_WhenReadingDoesntExist()
    {
        var httpClient = _factory.CreateClient();
        var pot = await PotTestDataManager.CreateItem(httpClient);
        _potsToDelete.Add(pot.Id);
        _plantsToDelete.Add(pot.PlantId);
        
        var item = ReadingTestDataManager.GenerateItem(pot.Id);
        _IdsToDelete.Add(item.Id);
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Detail.Should().Be($"Record with Id of '{item.Id}' does not exist.");
    }
    
    public async Task InitializeAsync()
    {
        
    }

    public async Task DisposeAsync()
    {
        var httpClient = _factory.CreateClient();

        await ReadingTestDataManager.DeleteItems(_IdsToDelete, httpClient);
        await PotTestDataManager.DeleteItems(_potsToDelete, httpClient);
        await PlantTestDataManager.DeleteItems(_plantsToDelete, httpClient);
    }
}