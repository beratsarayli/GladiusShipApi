using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("Users")]
public partial class User
{
    [Key]
    public Guid Ref { get; set; }

    public Guid RoleRef { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int IsPassive { get; set; }
}
