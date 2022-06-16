using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardenHub.Server.Data.Internal;

namespace GardenHub.Server.Data.Model;

[Table("Plant")]
public class Plant : EntityBase
{
    [Required]
    public string PlantName { get; set; } = default!;
}