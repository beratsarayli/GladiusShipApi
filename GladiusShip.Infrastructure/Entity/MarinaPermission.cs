using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MarinaPermission")]
public partial class MarinaPermission
{
    [Key]
    public Guid Ref { get; set; }

    public Guid PortRef { get; set; }

    public Guid ShipRef { get; set; }

    public int Status { get; set; }

    public DateTime CreateDate { get; set; }

    public int IsPassive { get; set; }

}
