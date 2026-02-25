using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("Ship")]
public partial class Ship
{
    [Key]
    public Guid Ref { get; set; }

    public Guid CustomerRef { get; set; }

    public Guid? CompanyRef { get; set; }

    public string Name { get; set; } = null!;

    public string HullId { get; set; } = null!;

    public string ImoNumber { get; set; } = null!;

    public string Flag { get; set; } = null!;

    public string RegistrationType { get; set; } = null!;

    public int IsPassive { get; set; }

}
