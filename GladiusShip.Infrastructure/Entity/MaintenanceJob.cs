using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MaintenanceJob")]
public partial class MaintenanceJob
{
    [Key]
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid? PersonalRef { get; set; }
    public string FormType { get; set; } = null!;
    public string ResponsiblePersonal { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime JobDate { get; set; }
    public int Status { get; set; }
    public int IsPassive { get; set; }
    public DateTime CreateDate { get; set; }
}