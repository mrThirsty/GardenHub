using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace GardenHub.Tests.Server;

public class GeneralServerTests : IClassFixture<ServerFactory>
{
    public GeneralServerTests(ServerFactory factory)
    {
        _factory = factory;
    }

    private readonly ServerFactory _factory;
    private string _baseUrl = "/system";
    
    [Fact]
    public async Task IsSystemUp()
    {
        var httpClient = _factory.CreateClient();

        var result = await httpClient.GetAsync(_baseUrl);
        var message = await result.Content.ReadAsStringAsync();

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        message.Should().BeEquivalentTo("\"The System is up\"");
    }
}