using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("Expertise")]
public partial class Expertise
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public string Description { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int Value { get; set; }

    public DateTime CreateDate { get; set; }

}
