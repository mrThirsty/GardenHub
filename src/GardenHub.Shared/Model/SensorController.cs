using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Shared.Model;

[Table("SensorController")]
public class SensorController : EntityBase
{
    public SensorController()
    {
        ReportingInterval = new(0, 12, 0, 0);
    }
    
    [Required]
    public string ControllerId { get; set; }
    
    public string? Description { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    public TimeSpan ReportingInterval { get; set; }
    
    [Required]
    [DefaultValue(true)]
    public bool Enabled { get; set; }
    
    public virtual  ICollection<Sensor> Sensors { get; set; }
}