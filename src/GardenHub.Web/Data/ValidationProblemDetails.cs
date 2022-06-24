using System.Text.Json.Serialization;

namespace GardenHub.Web.Data;

public class ValidationProblemDetails
{
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
    
    public new IDictionary<string, string[]> Errors { get; set; }  = new Dictionary<string, string[]>(StringComparer.Ordinal);
}