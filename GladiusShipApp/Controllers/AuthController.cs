using GladiusShip.Core.Models;
using GladiusShip.Core.Service.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GladiusShip.App.Controllers;

[Route("app/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success) return Unauthorized(result);
        return Ok(result);
    }
}