namespace GladiusShip.Core.Service.Ship;

public interface IShipService
{
    Task<ShipListResultModel> GetListAsync(Guid customerRef, CancellationToken cancellationToken = default);
    Task<ShipDetailResultModel> GetDetailAsync(Guid shipRef, Guid customerRef, CancellationToken cancellationToken = default);
    Task<ShipResultModel> CreateAsync(Guid customerRef, ShipCreateModel model, CancellationToken cancellationToken = default);
    Task<ShipResultModel> UpdateAsync(Guid shipRef, Guid customerRef, ShipUpdateModel model, CancellationToken cancellationToken = default);
    Task<ShipResultModel> SetPassiveAsync(Guid shipRef, Guid customerRef, CancellationToken cancellationToken = default);
    Task<ShipResultModel> SetActiveAsync(Guid shipRef, Guid customerRef, CancellationToken cancellationToken = default);
    Task<ShipResultModel> UpdateDocumentAsync(Guid shipRef, Guid customerRef, ShipDocumentModel model, CancellationToken cancellationToken = default);
    Task<ShipResultModel> UpdateMachineAsync(Guid shipRef, Guid customerRef, ShipMachineModel model, CancellationToken cancellationToken = default);
    Task<ShipResultModel> UpdateRegistrationAsync(Guid shipRef, Guid customerRef, ShipRegistrationModel model, CancellationToken cancellationToken = default);
    Task<ShipResultModel> AddPhotoAsync(Guid shipRef, Guid customerRef, string photo, string serialNumber, CancellationToken cancellationToken = default);
    Task<ShipResultModel> RemovePhotoAsync(Guid photoRef, Guid customerRef, CancellationToken cancellationToken = default);
}