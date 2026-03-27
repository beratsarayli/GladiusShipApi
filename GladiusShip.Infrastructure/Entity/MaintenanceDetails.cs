using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MaintenanceDetails")]
public partial class MaintenanceDetails
{
    [Key]
    public Guid Ref { get; set; }

    public Guid MaintenanceRef { get; set; }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int IsPassive { get; set; }

    public DateTime CreateDate { get; set; }

}
