using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("ShipRegistration")]
public partial class ShipRegistration
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public Guid PortRef { get; set; }

    public string RegistrationNumber { get; set; } = null!;

    public int RegistrationSize { get; set; }

    public int RegistrationWidth { get; set; }

    public int GrossTonilato { get; set; }

    public DateTime ShipCreateDate { get; set; }

}
