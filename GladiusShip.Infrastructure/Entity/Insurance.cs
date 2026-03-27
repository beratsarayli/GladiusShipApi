using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("Insurance")]
public partial class Insurance
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public Guid? PersonalRef { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int IsPassive { get; set; }

    public DateTime CreateDate { get; set; }

}
