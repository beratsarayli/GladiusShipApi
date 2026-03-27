using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MaintenanceJobItem")]
public partial class MaintenanceJobItem
{
    [Key]
    public Guid Ref { get; set; }
    public Guid JobRef { get; set; }
    public string WorkItem { get; set; } = null!;
    public string Material { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? WarrantyFile { get; set; }
    public int IsPassive { get; set; }
}