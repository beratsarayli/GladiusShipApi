using Amazon.Runtime;
using Amazon.S3;
using GladiusShip.Core.Service.Auth;
using GladiusShip.Core.Service.Insurance;
using GladiusShip.Core.Service.Maintenance;
using GladiusShip.Core.Service.Marina;
using GladiusShip.Core.Service.Port;
using GladiusShip.Core.Service.Role;
using GladiusShip.Core.Service.Ship;
using GladiusShip.Core.Service.Storage;
using GladiusShip.Core.Service.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GladiusShip.Core.Context;

public static class ServiceExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IInsuranceService, InsuranceService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IMaintenanceJobService, MaintenanceJobService>();
        services.AddScoped<IMarinaService, MarinaService>();
        services.AddScoped<IPortService, PortService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IShipService, ShipService>();
        services.AddScoped<IUserService, UserService>();

        var r2Endpoint = configuration["R2:Endpoint"];
        var r2AccessKey = configuration["R2:AccessKeyId"];
        var r2SecretKey = configuration["R2:SecretAccessKey"];
        if (!string.IsNullOrEmpty(r2Endpoint) && !string.IsNullOrEmpty(r2AccessKey) && !string.IsNullOrEmpty(r2SecretKey))
        {
            var creds = new BasicAWSCredentials(r2AccessKey, r2SecretKey);
            var s3Config = new AmazonS3Config { ServiceURL = r2Endpoint.TrimEnd('/') };
            services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(creds, s3Config));
            services.AddScoped<IR2StorageService, R2StorageService>();
        }

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(jwtSecret))
            throw new ArgumentNullException("Jwt:SecretKey", "JWT Secret key appsettings.json dosyasında bulunamadı!");

        var key = Encoding.UTF8.GetBytes(jwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers.Add("Token-Expired", "true");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context => Task.CompletedTask
            };
        });

        return services;
    }
}