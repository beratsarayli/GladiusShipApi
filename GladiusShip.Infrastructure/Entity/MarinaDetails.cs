using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("MarinaDetails")]
public partial class MarinaDetails
{
    [Key]
    public Guid Ref { get; set; }

    public Guid MarinaRef { get; set; }

    public string Description { get; set; } = null!;

    public int IsPassive { get; set; }

}
