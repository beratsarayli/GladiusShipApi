using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("RoleDetails")]
public partial class RoleDetail
{
    [Key]
    public Guid Ref { get; set; }

    public Guid RoleRef { get; set; }

    public string Permission { get; set; } = null!;
}
