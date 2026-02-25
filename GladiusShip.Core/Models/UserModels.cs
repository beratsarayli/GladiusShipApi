namespace GladiusShip.Core.Models;

public class UserListItemModel
{
    public Guid Ref { get; set; }
    public Guid RoleRef { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Mail { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public int IsPassive { get; set; }
    public bool IsActive => IsPassive != 1;
}

public class UserSetActiveResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}
