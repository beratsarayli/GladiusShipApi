namespace GladiusShip.Core.Models;

public class TokenModel
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}