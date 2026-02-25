using GladiusShip.Core.Service.Ship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GladiusShip.App.Controllers;

[Route("app/ship")]
[ApiController]
[Authorize]
public class ShipController : ControllerBase
{
    private readonly IShipService _shipService;

    public ShipController(IShipService shipService)
    {
        _shipService = shipService;
    }

    private Guid GetCustomerRef() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("list")]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var result = await _shipService.GetListAsync(GetCustomerRef(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}")]
    public async Task<IActionResult> GetDetail(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _shipService.GetDetailAsync(shipRef, GetCustomerRef(), cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ShipCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _shipService.CreateAsync(GetCustomerRef(), model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/update")]
    public async Task<IActionResult> Update(Guid shipRef, [FromBody] ShipUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _shipService.UpdateAsync(shipRef, GetCustomerRef(), model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/passive")]
    public async Task<IActionResult> SetPassive(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _shipService.SetPassiveAsync(shipRef, GetCustomerRef(), cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/active")]
    public async Task<IActionResult> SetActive(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _shipService.SetActiveAsync(shipRef, GetCustomerRef(), cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/document")]
    public async Task<IActionResult> UpdateDocument(Guid shipRef, [FromBody] ShipDocumentModel model, CancellationToken cancellationToken)
    {
        var result = await _shipService.UpdateDocumentAsync(shipRef, GetCustomerRef(), model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/machine")]
    public async Task<IActionResult> UpdateMachine(Guid shipRef, [FromBody] ShipMachineModel model, CancellationToken cancellationToken)
    {
        var result = await _shipService.UpdateMachineAsync(shipRef, GetCustomerRef(), model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/registration")]
    public async Task<IActionResult> UpdateRegistration(Guid shipRef, [FromBody] ShipRegistrationModel model, CancellationToken cancellationToken)
    {
        var result = await _shipService.UpdateRegistrationAsync(shipRef, GetCustomerRef(), model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/photo")]
    public async Task<IActionResult> AddPhoto(Guid shipRef, [FromBody] ShipPhotoInputModel model, CancellationToken cancellationToken)
    {
        var result = await _shipService.AddPhotoAsync(shipRef, GetCustomerRef(), model.Photo, model.SerialNumber, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("photo/{photoRef:guid}")]
    public async Task<IActionResult> RemovePhoto(Guid photoRef, CancellationToken cancellationToken)
    {
        var result = await _shipService.RemovePhotoAsync(photoRef, GetCustomerRef(), cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}