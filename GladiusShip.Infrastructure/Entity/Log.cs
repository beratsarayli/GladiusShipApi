using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("Log")]
public partial class Log
{
    [Key]
    public Guid Ref { get; set; }

    public string Name { get; set; } = null!;

    public int Level { get; set; } 

    public DateTime CreateDate { get; set; }
}
