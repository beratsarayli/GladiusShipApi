using GladiusShip.Core.Service.Marina;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GladiusShip.App.Controllers;

[Route("app/marina")]
[ApiController]
[Authorize]
public class MarinaController : ControllerBase
{
    private readonly IMarinaService _marinaService;

    public MarinaController(IMarinaService marinaService)
    {
        _marinaService = marinaService;
    }

    [HttpGet("{portRef:guid}/list")]
    public async Task<IActionResult> GetList(Guid portRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.GetListAsync(portRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{portRef:guid}/{marinaRef:guid}")]
    public async Task<IActionResult> GetDetail(Guid portRef, Guid marinaRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.GetDetailAsync(marinaRef, portRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/create")]
    public async Task<IActionResult> Create(Guid portRef, [FromBody] MarinaCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _marinaService.CreateAsync(portRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{portRef:guid}/{marinaRef:guid}/update")]
    public async Task<IActionResult> Update(Guid portRef, Guid marinaRef, [FromBody] MarinaUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _marinaService.UpdateAsync(marinaRef, portRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/{marinaRef:guid}/active")]
    public async Task<IActionResult> SetActive(Guid portRef, Guid marinaRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.SetActiveAsync(marinaRef, portRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/{marinaRef:guid}/passive")]
    public async Task<IActionResult> SetPassive(Guid portRef, Guid marinaRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.SetPassiveAsync(marinaRef, portRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/{marinaRef:guid}/detail/add")]
    public async Task<IActionResult> AddDetail(Guid portRef, Guid marinaRef, [FromBody] string description, CancellationToken cancellationToken)
    {
        var result = await _marinaService.AddDetailAsync(marinaRef, portRef, description, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{portRef:guid}/detail/{detailRef:guid}/update")]
    public async Task<IActionResult> UpdateDetail(Guid portRef, Guid detailRef, [FromBody] string description, CancellationToken cancellationToken)
    {
        var result = await _marinaService.UpdateDetailAsync(detailRef, portRef, description, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/detail/{detailRef:guid}/active")]
    public async Task<IActionResult> SetDetailActive(Guid portRef, Guid detailRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.SetDetailActiveAsync(detailRef, portRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/detail/{detailRef:guid}/passive")]
    public async Task<IActionResult> SetDetailPassive(Guid portRef, Guid detailRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.SetDetailPassiveAsync(detailRef, portRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{portRef:guid}/detail/{detailRef:guid}/remove")]
    public async Task<IActionResult> RemoveDetail(Guid portRef, Guid detailRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.RemoveDetailAsync(detailRef, portRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/ship/{shipRef:guid}/entry")]
    public async Task<IActionResult> ShipEntry(Guid portRef, Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.ShipEntryAsync(portRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/ship/{shipRef:guid}/exit")]
    public async Task<IActionResult> ShipExit(Guid portRef, Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.ShipExitAsync(portRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("ship/{shipRef:guid}/roads")]
    public async Task<IActionResult> GetShipRoads(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.GetShipRoadsAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{portRef:guid}/roads")]
    public async Task<IActionResult> GetPortRoads(Guid portRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.GetPortRoadsAsync(portRef, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/ship/{shipRef:guid}/permission/grant")]
    public async Task<IActionResult> GrantPermission(Guid portRef, Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.GrantPermissionAsync(portRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{portRef:guid}/ship/{shipRef:guid}/permission/revoke")]
    public async Task<IActionResult> RevokePermission(Guid portRef, Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.RevokePermissionAsync(portRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{portRef:guid}/permissions")]
    public async Task<IActionResult> GetPermissions(Guid portRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.GetPermissionsAsync(portRef, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{marinaRef:guid}/price/add")]
    public async Task<IActionResult> AddPrice(Guid marinaRef, [FromBody] MarinaPriceCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _marinaService.AddPriceAsync(marinaRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{marinaRef:guid}/prices")]
    public async Task<IActionResult> GetPrices(Guid marinaRef, CancellationToken cancellationToken)
    {
        var result = await _marinaService.GetPricesAsync(marinaRef, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{marinaRef:guid}/price/{priceRef:guid}/payment-status")]
    public async Task<IActionResult> UpdatePricePaymentStatus(
    Guid marinaRef,
    Guid priceRef,
    [FromBody] MarinaPricePaymentUpdateModel model,
    CancellationToken cancellationToken)
    {
        var result = await _marinaService.UpdatePricePaymentStatusAsync(marinaRef, priceRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

}