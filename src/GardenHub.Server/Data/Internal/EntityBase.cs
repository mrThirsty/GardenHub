using System.ComponentModel.DataAnnotations;

namespace GardenHub.Server.Data.Internal;

public class EntityBase
{
    [Required]
    [Key]
    public Guid Id { get; set; } = Guid.Empty;
}