using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GladiusShip.Infrastructure.Entity;

[Table("ShipDocument")]
public partial class ShipDocument
{
    [Key]
    public Guid Ref { get; set; }

    public Guid ShipRef { get; set; }

    public DateTime PermitValidity { get; set; }

    public DateTime İnsuranceExpire { get; set; }

    public string? RadioCallSign { get; set; } = null!;

    public string? MMSINumber { get; set; } = null!;

    public string? CEDocumentNumber { get; set; } = null!;

}
