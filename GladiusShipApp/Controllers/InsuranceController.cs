using GladiusShip.Core.Service.Insurance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GladiusShip.App.Controllers;

[Route("app/insurance")]
[ApiController]
[Authorize]
public class InsuranceController : ControllerBase
{
    private readonly IInsuranceService _insuranceService;

    public InsuranceController(IInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpGet("{shipRef:guid}/list")]
    public async Task<IActionResult> GetList(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.GetListAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/{insuranceRef:guid}")]
    public async Task<IActionResult> GetDetail(Guid shipRef, Guid insuranceRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.GetDetailAsync(insuranceRef, shipRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/create")]
    public async Task<IActionResult> Create(Guid shipRef, [FromBody] InsuranceCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.CreateAsync(shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/{insuranceRef:guid}/update")]
    public async Task<IActionResult> Update(Guid shipRef, Guid insuranceRef, [FromBody] InsuranceUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.UpdateAsync(insuranceRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{insuranceRef:guid}/active")]
    public async Task<IActionResult> SetActive(Guid shipRef, Guid insuranceRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.SetActiveAsync(insuranceRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{insuranceRef:guid}/passive")]
    public async Task<IActionResult> SetPassive(Guid shipRef, Guid insuranceRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.SetPassiveAsync(insuranceRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/expertise/list")]
    public async Task<IActionResult> GetExpertiseList(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.GetExpertiseListAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/expertise/{expertiseRef:guid}")]
    public async Task<IActionResult> GetExpertiseDetail(Guid shipRef, Guid expertiseRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.GetExpertiseDetailAsync(expertiseRef, shipRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/expertise/create")]
    public async Task<IActionResult> CreateExpertise(Guid shipRef, [FromBody] ExpertiseCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.CreateExpertiseAsync(shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/expertise/{expertiseRef:guid}/update")]
    public async Task<IActionResult> UpdateExpertise(Guid shipRef, Guid expertiseRef, [FromBody] ExpertiseUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.UpdateExpertiseAsync(expertiseRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/details/list")]
    public async Task<IActionResult> GetInsuranceDetailsList(Guid shipRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.GetInsuranceDetailsListAsync(shipRef, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/details/{detailRef:guid}")]
    public async Task<IActionResult> GetInsuranceDetailsDetail(Guid shipRef, Guid detailRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.GetInsuranceDetailsDetailAsync(detailRef, shipRef, cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/details/create")]
    public async Task<IActionResult> CreateInsuranceDetails(Guid shipRef, [FromBody] InsuranceDetailsCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.CreateInsuranceDetailsAsync(shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/details/{detailRef:guid}/update")]
    public async Task<IActionResult> UpdateInsuranceDetails(Guid shipRef, Guid detailRef, [FromBody] InsuranceDetailsUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.UpdateInsuranceDetailsAsync(detailRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{shipRef:guid}/{insuranceRef:guid}/discount/list")]
    public async Task<IActionResult> GetDiscountList(Guid shipRef, Guid insuranceRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.GetDiscountListAsync(shipRef, insuranceRef, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/{insuranceRef:guid}/discount/create")]
    public async Task<IActionResult> CreateDiscount(Guid shipRef, Guid insuranceRef, [FromBody] InsuranceDiscountCreateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.CreateDiscountAsync(shipRef, insuranceRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{shipRef:guid}/discount/{discountRef:guid}/update")]
    public async Task<IActionResult> UpdateDiscount(Guid shipRef, Guid discountRef, [FromBody] InsuranceDiscountUpdateModel model, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.UpdateDiscountAsync(discountRef, shipRef, model, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/discount/{discountRef:guid}/active")]
    public async Task<IActionResult> SetDiscountActive(Guid shipRef, Guid discountRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.SetDiscountActiveAsync(discountRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{shipRef:guid}/discount/{discountRef:guid}/passive")]
    public async Task<IActionResult> SetDiscountPassive(Guid shipRef, Guid discountRef, CancellationToken cancellationToken)
    {
        var result = await _insuranceService.SetDiscountPassiveAsync(discountRef, shipRef, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

}