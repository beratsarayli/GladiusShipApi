using GladiusShip.Core.Service.Maintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GladiusShip.App.Controllers;

[Route("app/maintenance")]
[ApiController]
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceController(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }

    private Guid GetCustomerRef() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{shipRef:guid}/list")]
    public async Task<IActionResult> GetList(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetListAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/{maintenanceRef:guid}")]
    public async Task<IActionResult> GetDetail(Guid shipRef, Guid maintenanceRef, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetDetailAsync(maintenanceRef, shipRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/create")]
    public async Task<IActionResult> Create(Guid shipRef, [FromBody] MaintenanceCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.CreateAsync(shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/{maintenanceRef:guid}/update")]
    public async Task<IActionResult> Update(Guid shipRef, Guid maintenanceRef, [FromBody] MaintenanceUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.UpdateAsync(maintenanceRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{maintenanceRef:guid}/active")]
    public async Task<IActionResult> SetActive(Guid shipRef, Guid maintenanceRef, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.SetActiveAsync(maintenanceRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{maintenanceRef:guid}/passive")]
    public async Task<IActionResult> SetPassive(Guid shipRef, Guid maintenanceRef, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.SetPassiveAsync(maintenanceRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{maintenanceRef:guid}/detail/add")]
    public async Task<IActionResult> AddDetail(Guid shipRef, Guid maintenanceRef, [FromBody] MaintenanceDetailCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.AddDetailAsync(maintenanceRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/detail/{detailRef:guid}/update")]
    public async Task<IActionResult> UpdateDetail(Guid shipRef, Guid detailRef, [FromBody] MaintenanceDetailCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.UpdateDetailAsync(detailRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/detail/{detailRef:guid}/active")]
    public async Task<IActionResult> SetDetailActive(Guid shipRef, Guid detailRef, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.SetDetailActiveAsync(detailRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/detail/{detailRef:guid}/passive")]
    public async Task<IActionResult> SetDetailPassive(Guid shipRef, Guid detailRef, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.SetDetailPassiveAsync(detailRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{shipRef:guid}/detail/{detailRef:guid}/remove")]
    public async Task<IActionResult> RemoveDetail(Guid shipRef, Guid detailRef, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.RemoveDetailAsync(detailRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}