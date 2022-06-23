using System.ComponentModel.DataAnnotations;

namespace GardenHub.Shared.Model.Internal;

public class EntityBase
{
    [Required]
    [Key]
    public Guid Id { get; set; } = Guid.Empty;
}