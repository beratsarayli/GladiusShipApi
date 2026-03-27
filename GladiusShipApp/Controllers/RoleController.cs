using GladiusShip.Core.Service.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GladiusShip.App.Controllers;

[Route("app/role")]
[ApiController]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var result = await _roleService.GetListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{roleRef:guid}")]
    public async Task<IActionResult> GetDetail(Guid roleRef, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetDetailAsync(roleRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] RoleCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _roleService.CreateAsync(model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{roleRef:guid}/update")]
    public async Task<IActionResult> Update(Guid roleRef, [FromBody] RoleUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _roleService.UpdateAsync(roleRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{roleRef:guid}/active")]
    public async Task<IActionResult> SetActive(Guid roleRef, CancellationToken cancellationToken)
    {
        var result = await _roleService.SetActiveAsync(roleRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{roleRef:guid}/passive")]
    public async Task<IActionResult> SetPassive(Guid roleRef, CancellationToken cancellationToken)
    {
        var result = await _roleService.SetPassiveAsync(roleRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{roleRef:guid}/permissions")]
    public async Task<IActionResult> UpdatePermissions(Guid roleRef, [FromBody] List<string> permissions, CancellationToken cancellationToken)
    {
        var result = await _roleService.UpdatePermissionsAsync(roleRef, permissions, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}