using GladiusShip.Core.Service.Maintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GladiusShip.App.Controllers;

[Route("app/maintenancejob")]
[ApiController]
[Authorize]
public class MaintenanceJobController : ControllerBase
{
    private readonly IMaintenanceJobService _jobService;

    public MaintenanceJobController(IMaintenanceJobService jobService)
    {
        _jobService = jobService;
    }

    private Guid GetCustomerRef() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{shipRef:guid}/list")]
    public async Task<IActionResult> GetList(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.GetListAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/active")]
    public async Task<IActionResult> GetActiveJobs(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.GetActiveJobsAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/completed")]
    public async Task<IActionResult> GetCompletedJobs(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.GetCompletedJobsAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/{jobRef:guid}")]
    public async Task<IActionResult> GetDetail(Guid shipRef, Guid jobRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.GetDetailAsync(jobRef, shipRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/draft")]
    public async Task<IActionResult> CreateDraft(Guid shipRef, [FromBody] JobCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.CreateDraftAsync(shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/{jobRef:guid}/update")]
    public async Task<IActionResult> Update(Guid shipRef, Guid jobRef, [FromBody] JobUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.UpdateAsync(jobRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{jobRef:guid}/submit")]
    public async Task<IActionResult> Submit(Guid shipRef, Guid jobRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.SubmitForApprovalAsync(jobRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{jobRef:guid}/approve")]
    public async Task<IActionResult> Approve(Guid shipRef, Guid jobRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.ApproveAsync(jobRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{jobRef:guid}/reject")]
    public async Task<IActionResult> Reject(Guid shipRef, Guid jobRef, [FromBody] string reason, CancellationToken cancellationToken)
    {
        var result = await _jobService.RejectAsync(jobRef, shipRef, reason, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{jobRef:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid shipRef, Guid jobRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.CancelAsync(jobRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{jobRef:guid}/item/add")]
    public async Task<IActionResult> AddItem(Guid shipRef, Guid jobRef, [FromBody] JobItemModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.AddItemAsync(jobRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{shipRef:guid}/item/{itemRef:guid}/remove")]
    public async Task<IActionResult> RemoveItem(Guid shipRef, Guid itemRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.RemoveItemAsync(itemRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/{jobRef:guid}/risk")]
    public async Task<IActionResult> SetRisk(Guid shipRef, Guid jobRef, [FromBody] JobRiskModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.SetRiskAsync(jobRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/{jobRef:guid}/cost")]
    public async Task<IActionResult> SetCost(Guid shipRef, Guid jobRef, [FromBody] JobCostModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.SetCostAsync(jobRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{jobRef:guid}/photo/add")]
    public async Task<IActionResult> AddPhoto(Guid shipRef, Guid jobRef, [FromBody] JobPhotoRequest request, CancellationToken cancellationToken)
    {
        var result = await _jobService.AddPhotoAsync(jobRef, shipRef, request.PhotoPath, request.Category, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{shipRef:guid}/photo/{photoRef:guid}/remove")]
    public async Task<IActionResult> RemovePhoto(Guid shipRef, Guid photoRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.RemovePhotoAsync(photoRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/{jobRef:guid}/action")]
    public async Task<IActionResult> SetAction(Guid shipRef, Guid jobRef, [FromBody] JobActionModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.SetActionAsync(jobRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/{maintenanceRef:guid}/costdocument/list")]
    public async Task<IActionResult> GetCostDocumentList(Guid shipRef, Guid maintenanceRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.GetCostDocumentListAsync(shipRef, maintenanceRef, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{maintenanceRef:guid}/costdocument/create")]
    public async Task<IActionResult> CreateCostDocument(Guid shipRef, Guid maintenanceRef, [FromBody] CostDocumentCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.CreateCostDocumentAsync(shipRef, maintenanceRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/costdocument/{documentRef:guid}/update")]
    public async Task<IActionResult> UpdateCostDocument(Guid shipRef, Guid documentRef, [FromBody] CostDocumentUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _jobService.UpdateCostDocumentAsync(documentRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/costdocument/{documentRef:guid}/active")]
    public async Task<IActionResult> SetCostDocumentActive(Guid shipRef, Guid documentRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.SetCostDocumentActiveAsync(documentRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/costdocument/{documentRef:guid}/passive")]
    public async Task<IActionResult> SetCostDocumentPassive(Guid shipRef, Guid documentRef, CancellationToken cancellationToken)
    {
        var result = await _jobService.SetCostDocumentPassiveAsync(documentRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}

#region Models

public class JobPhotoRequest
{
    public string PhotoPath { get; set; } = null!;
    public string Category { get; set; } = null!;
}

#endregion