using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Shared.Model;

[Table("SensorController")]
public class SensorController : EntityBase
{
    [Required]
    public string ControllerId { get; set; }
    
    public string? Description { get; set; }
    
    public virtual  ICollection<Sensor> Sensors { get; set; }
}