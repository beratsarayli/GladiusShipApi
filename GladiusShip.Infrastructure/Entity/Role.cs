using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("Role")]
public partial class Role
{
    [Key]
    public Guid Ref { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public int IsPassive { get; set; }
}
