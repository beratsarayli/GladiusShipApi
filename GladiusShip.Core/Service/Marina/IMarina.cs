namespace GladiusShip.Core.Service.Marina;

public interface IMarinaService
{
    Task<MarinaListResultModel> GetListAsync(Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaDetailResultModel> GetDetailAsync(Guid marinaRef, Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> CreateAsync(Guid portRef, MarinaCreateModel model, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> UpdateAsync(Guid marinaRef, Guid portRef, MarinaUpdateModel model, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> SetActiveAsync(Guid marinaRef, Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> SetPassiveAsync(Guid marinaRef, Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> AddDetailAsync(Guid marinaRef, Guid portRef, string description, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> UpdateDetailAsync(Guid detailRef, Guid portRef, string description, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> SetDetailActiveAsync(Guid detailRef, Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> SetDetailPassiveAsync(Guid detailRef, Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> RemoveDetailAsync(Guid detailRef, Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> ShipEntryAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> ShipExitAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MarinaRoadListResultModel> GetShipRoadsAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<MarinaRoadListResultModel> GetPortRoadsAsync(Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> GrantPermissionAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> RevokePermissionAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MarinaPermissionListResultModel> GetPermissionsAsync(Guid portRef, CancellationToken cancellationToken = default);
    Task<MarinaResultModel> AddPriceAsync(Guid marinaRef, MarinaPriceCreateModel model, CancellationToken cancellationToken = default);
    Task<MarinaPriceListResultModel> GetPricesAsync(Guid marinaRef, CancellationToken cancellationToken = default);

    Task<MarinaResultModel> UpdatePricePaymentStatusAsync(
    Guid marinaRef,
    Guid priceRef,
    MarinaPricePaymentUpdateModel model,
    CancellationToken cancellationToken = default);
}