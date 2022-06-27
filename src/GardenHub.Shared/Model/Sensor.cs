using System.ComponentModel.DataAnnotations;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Shared.Model;

public class Sensor : EntityBase
{
    [Required]
    public string SensorName { get; set; } 
}