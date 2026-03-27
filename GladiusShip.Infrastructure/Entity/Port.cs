using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("Port")]
public partial class Port
{
    [Key]
    public Guid Ref { get; set; }

    public Guid? CompanyRef { get; set; }

    public string Name { get; set; } = null!;

    public int IsPassive { get; set; }

}
