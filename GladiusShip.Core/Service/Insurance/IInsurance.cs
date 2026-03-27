namespace GladiusShip.Core.Service.Insurance;

public interface IInsuranceService
{
    Task<InsuranceListResultModel> GetListAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<InsuranceDetailResultModel> GetDetailAsync(Guid insuranceRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<InsuranceResultModel> CreateAsync(Guid shipRef, InsuranceCreateModel model, CancellationToken cancellationToken = default);
    Task<InsuranceResultModel> UpdateAsync(Guid insuranceRef, Guid shipRef, InsuranceUpdateModel model, CancellationToken cancellationToken = default);
    Task<InsuranceResultModel> SetActiveAsync(Guid insuranceRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<InsuranceResultModel> SetPassiveAsync(Guid insuranceRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<ExpertiseListResultModel> GetExpertiseListAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<ExpertiseDetailResultModel> GetExpertiseDetailAsync(Guid expertiseRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<ExpertiseResultModel> CreateExpertiseAsync(Guid shipRef, ExpertiseCreateModel model, CancellationToken cancellationToken = default);
    Task<ExpertiseResultModel> UpdateExpertiseAsync(Guid expertiseRef, Guid shipRef, ExpertiseUpdateModel model, CancellationToken cancellationToken = default);

    Task<InsuranceDetailsListResultModel> GetInsuranceDetailsListAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<InsuranceDetailsDetailResultModel> GetInsuranceDetailsDetailAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<InsuranceDetailsResultModel> CreateInsuranceDetailsAsync(Guid shipRef, InsuranceDetailsCreateModel model, CancellationToken cancellationToken = default);
    Task<InsuranceDetailsResultModel> UpdateInsuranceDetailsAsync(Guid detailRef, Guid shipRef, InsuranceDetailsUpdateModel model, CancellationToken cancellationToken = default);

    Task<InsuranceDiscountListResultModel> GetDiscountListAsync(Guid shipRef, Guid insuranceRef, CancellationToken cancellationToken = default);
    Task<InsuranceDiscountResultModel> CreateDiscountAsync(Guid shipRef, Guid insuranceRef, InsuranceDiscountCreateModel model, CancellationToken cancellationToken = default);
    Task<InsuranceDiscountResultModel> UpdateDiscountAsync(Guid discountRef, Guid shipRef, InsuranceDiscountUpdateModel model, CancellationToken cancellationToken = default);
    Task<InsuranceDiscountResultModel> SetDiscountActiveAsync(Guid discountRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<InsuranceDiscountResultModel> SetDiscountPassiveAsync(Guid discountRef, Guid shipRef, CancellationToken cancellationToken = default);
}