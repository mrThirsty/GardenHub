using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Shared.Model;

[Table("Pot")]
public class Pot : EntityBase
{
    [Required] 
    public string PotName { get; set; } = default!;
    
    [Required]
    [ForeignKey(nameof(PlantId))]
    public Guid PlantId { get; set; }
    
    [Required]
    public DateTime DatePlanted { get; set; } = DateTime.Now;
    
    public string? Notes { get; set; }
    
    public virtual Plant Plant { get; set; }
}