using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("ShipMachine")]
public partial class ShipMachine
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public string MachineBrand { get; set; } = null!;

    public string MachineModel { get; set; } = null!;

    public string MachineSerial { get; set; } = null!;

    public int Power { get; set; }

    public int EngineClock { get; set; }
}
