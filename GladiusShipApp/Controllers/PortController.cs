using GladiusShip.Core.Service.Port;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GladiusShip.App.Controllers;

[Route("app/port")]
[ApiController]
[Authorize]
public class PortController : ControllerBase
{
    private readonly IPortService _portService;

    public PortController(IPortService portService)
    {
        _portService = portService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var result = await _portService.GetListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{portRef:guid}")]
    public async Task<IActionResult> GetDetail(Guid portRef, CancellationToken cancellationToken)
    {
        var result = await _portService.GetDetailAsync(portRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] string name, CancellationToken cancellationToken)
    {
        var result = await _portService.CreateAsync(name, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{portRef:guid}/update")]
    public async Task<IActionResult> Update(Guid portRef, [FromBody] string name, CancellationToken cancellationToken)
    {
        var result = await _portService.UpdateAsync(portRef, name, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/active")]
    public async Task<IActionResult> SetActive(Guid portRef, CancellationToken cancellationToken)
    {
        var result = await _portService.SetActiveAsync(portRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/passive")]
    public async Task<IActionResult> SetPassive(Guid portRef, CancellationToken cancellationToken)
    {
        var result = await _portService.SetPassiveAsync(portRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}