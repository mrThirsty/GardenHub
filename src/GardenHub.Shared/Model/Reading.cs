using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Shared.Model;

[Table("Reading")]
public class Reading : EntityBase
{
    [Required]
    [ForeignKey(nameof(PotId))]
    public Guid PotId { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; }
    
    [Required]
    public double SoilMoistureReading { get; set; }
    
    public virtual  Pot Pot { get; set; }
}