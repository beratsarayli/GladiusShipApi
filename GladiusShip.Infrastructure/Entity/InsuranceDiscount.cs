using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("InsuranceDiscount")]
public partial class InsuranceDiscount
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public Guid InsuranceRef { get; set; }

    public int Value { get; set; }

    public string Description { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int IsPassive { get; set; }

    public DateTime CreateDate { get; set; }

}
