using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MaintenanceJobCost")]
public partial class MaintenanceJobCost
{
    [Key]
    public Guid Ref { get; set; }
    public Guid JobRef { get; set; }
    public decimal PartCost { get; set; }
    public decimal LaborCost { get; set; }
    public string Currency { get; set; } = null!;
    public string? InvoiceFile { get; set; }
}