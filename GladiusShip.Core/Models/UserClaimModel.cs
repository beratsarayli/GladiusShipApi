namespace GladiusShip.Core.Models;

public class UserClaimModel
{
    public Guid Ref { get; set; }
    public Guid RoleRef { get; set; }
    public string Mail { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsSuperAdmin { get; set; }
}