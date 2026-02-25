using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("ShipPhoto")]
public partial class ShipPhoto
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public string Photo { get; set; } = null!;

    public string SerialNumber { get; set; } = null!;
}
