using GladiusShip.Core.Models;

namespace GladiusShip.Core.Service.Auth;

public interface IAuthService
{
    #region Auth
    Task<AuthResponseModel> LoginAsync(LoginRequestModel request);
    #endregion

    #region Token
    string GenerateToken(UserClaimModel user);
    UserClaimModel? ValidateToken(string token);
    #endregion

    #region Password
    string HashPassword(string password);
    #endregion
}