namespace GladiusShip.Core.Models;

public class AuthResponseModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool IsSuperAdmin { get; set; }
    public TokenModel? Token { get; set; }
}