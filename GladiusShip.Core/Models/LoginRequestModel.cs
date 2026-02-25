namespace GladiusShip.Core.Models;

public class LoginRequestModel
{
    public string Mail { get; set; } = null!;
    public string Password { get; set; } = null!;
}