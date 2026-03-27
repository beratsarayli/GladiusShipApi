using GladiusShip.Core.Service.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GladiusShip.App.Controllers;

[Route("app/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var list = await _userService.GetListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] UserAddRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.AddAsync(request.RoleRef, request.Name, request.Surname, request.Phone, request.Mail, request.Password, request.Description, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{userRef:guid}/update")]
    public async Task<IActionResult> Update(Guid userRef, [FromBody] UserUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(userRef, request.RoleRef, request.Name, request.Surname, request.Phone, request.Mail, request.Description, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{userRef:guid}/password")]
    public async Task<IActionResult> UpdatePassword(Guid userRef, [FromBody] UserPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdatePasswordAsync(userRef, request.NewPassword, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{userRef:guid}/active")]
    public async Task<IActionResult> SetActive(Guid userRef, CancellationToken cancellationToken)
    {
        var result = await _userService.SetActiveAsync(userRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{userRef:guid}/passive")]
    public async Task<IActionResult> SetPassive(Guid userRef, CancellationToken cancellationToken)
    {
        var result = await _userService.SetPassiveAsync(userRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}

#region Models

public class UserAddRequest
{
    public Guid RoleRef { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Mail { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class UserUpdateRequest
{
    public Guid RoleRef { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Mail { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class UserPasswordRequest
{
    public string NewPassword { get; set; } = null!;
}

#endregion