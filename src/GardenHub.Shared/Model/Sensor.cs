using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Shared.Model;

public class Sensor : EntityBase
{
    [Required]
    public string SensorName { get; set; }
    
    [Required]
    [ForeignKey(nameof(SensorControllerId))]
    public Guid SensorControllerId { get; set; }
    
    public virtual SensorController SensorController { get; set; }
}