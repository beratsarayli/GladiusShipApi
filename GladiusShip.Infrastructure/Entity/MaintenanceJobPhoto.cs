using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MaintenanceJobPhoto")]
public partial class MaintenanceJobPhoto
{
    [Key]
    public Guid Ref { get; set; }
    public Guid JobRef { get; set; }
    public string PhotoPath { get; set; } = null!;
    public string Category { get; set; } = null!;
    public DateTime CreateDate { get; set; }
}