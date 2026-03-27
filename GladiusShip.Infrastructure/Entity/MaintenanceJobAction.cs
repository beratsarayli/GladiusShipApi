using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MaintenanceJobAction")]
public partial class MaintenanceJobAction
{
    [Key]
    public Guid Ref { get; set; }
    public Guid JobRef { get; set; }
    public string? MasterComment { get; set; }
    public DateTime? NextActionDate { get; set; }
    public int? NextActionHour { get; set; }
}