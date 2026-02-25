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
    public async Task<ActionResult<IReadOnlyList<UserListItemModel>>> GetList(CancellationToken cancellationToken)
    {
        var list = await _userService.GetListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpPost("add")]
    public async Task<ActionResult<UserSetActiveResultModel>> Add([FromBody] UserAddRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.AddAsync(request.RoleRef, request.Name, request.Surname, request.Phone, request.Mail, request.Password, request.Description, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{userRef:guid}/active")]
    public async Task<ActionResult<UserSetActiveResultModel>> SetActive(Guid userRef, CancellationToken cancellationToken)
    {
        var result = await _userService.SetActiveAsync(userRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{userRef:guid}/passive")]
    public async Task<ActionResult<UserSetActiveResultModel>> SetPassive(Guid userRef, CancellationToken cancellationToken)
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

#endregion