using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MaintenanceJobRisk")]
public partial class MaintenanceJobRisk
{
    [Key]
    public Guid Ref { get; set; }
    public Guid JobRef { get; set; }
    public string OperationType { get; set; } = null!;
    public int? EngineHourBefore { get; set; }
    public int? EngineHourAfter { get; set; }
    public bool HasWaste { get; set; }
    public string? WasteDetail { get; set; }
}