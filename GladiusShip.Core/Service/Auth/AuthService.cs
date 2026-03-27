using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GladiusShip.Core.Models;
using GladiusShip.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GladiusShip.Core.Service.Auth;

public class AuthService : IAuthService
{

    private readonly GladiusShipContext _db;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;


    public AuthService(GladiusShipContext db, IConfiguration configuration)
    {
        _db = db;
        _jwtSecret = configuration["Jwt:SecretKey"]!;
        _jwtIssuer = configuration["Jwt:Issuer"]!;
        _jwtAudience = configuration["Jwt:Audience"]!;
    }


    #region Auth

    public async Task<AuthResponseModel> LoginAsync(LoginRequestModel request)
    {
        try
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Mail == request.Mail && x.IsPassive != 1);

            if (user == null)
                return new AuthResponseModel { Success = false, Message = "Kullanıcı bulunamadı veya hesap pasif." };

            if (!VerifyPassword(request.Password, user.Password))
                return new AuthResponseModel { Success = false, Message = "Şifre hatalı." };

            var isSuperAdmin = user.IsPassive == 2;

            var token = GenerateToken(new UserClaimModel
            {
                Ref = user.Ref,
                RoleRef = user.RoleRef,
                Mail = user.Mail,
                FullName = $"{user.Name} {user.Surname}",
                IsSuperAdmin = isSuperAdmin
            });

            return new AuthResponseModel
            {
                Success = true,
                IsSuperAdmin = isSuperAdmin,
                Token = new TokenModel
                {
                    AccessToken = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(8)
                }
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseModel { Success = false, Message = $"Hata: {ex.Message}" };
        }
    }

    #endregion

    #region Token

    public string GenerateToken(UserClaimModel user)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Ref.ToString()),
            new Claim(ClaimTypes.Email, user.Mail),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("RoleRef", user.RoleRef.ToString()),
            new Claim("IsSuperAdmin", user.IsSuperAdmin.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(tokenDescriptor));
    }

    public UserClaimModel? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var handler = new JwtSecurityTokenHandler();

            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;

            return new UserClaimModel
            {
                Ref = Guid.Parse(jwt.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
                Mail = jwt.Claims.First(x => x.Type == ClaimTypes.Email).Value,
                FullName = jwt.Claims.First(x => x.Type == ClaimTypes.Name).Value,
                RoleRef = Guid.Parse(jwt.Claims.First(x => x.Type == "RoleRef").Value)
            };
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Password

    public string HashPassword(string password) => BCryptNet.HashPassword(password);

    private bool VerifyPassword(string input, string stored)
    {
        if (string.IsNullOrWhiteSpace(stored)) return false;
        if (stored.StartsWith("$2")) return BCryptNet.Verify(input, stored);
        return input == stored;
    }

    #endregion
}