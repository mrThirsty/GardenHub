using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardenHub.Server.Data.Internal;

namespace GardenHub.Server.Data.Model;

[Table("Plant")]
public class Plant : EntityBase
{
    [Required]
    public string PlantName { get; set; } = default!;
    [Required]
    public double RequiredSoilMoisture { get; set; }
    [Required]
    public LightLevel RequiredSun { get; set; }
    
    public string Description { get; set; }
    
    public virtual ICollection<Pot> Pots { get; set; } = default!;
}