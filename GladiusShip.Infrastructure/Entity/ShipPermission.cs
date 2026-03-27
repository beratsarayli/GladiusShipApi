using GladiusShip.Infrastructure.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace GladiusShip.Infrastructure.Entity;

[Table("ShipPermission")]
public partial class ShipPermission
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public Guid PersonalRef { get; set; }

    public string Permission { get; set; }

    public DateTime CreateDate { get; set; }

    public int IsPassive { get; set; }
}
