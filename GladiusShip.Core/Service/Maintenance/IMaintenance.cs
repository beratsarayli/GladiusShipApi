namespace GladiusShip.Core.Service.Maintenance;

public interface IMaintenanceService
{
    Task<MaintenanceListResultModel> GetListAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<MaintenanceDetailResultModel> GetDetailAsync(Guid maintenanceRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> CreateAsync(Guid shipRef, MaintenanceCreateModel model, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> UpdateAsync(Guid maintenanceRef, Guid shipRef, MaintenanceUpdateModel model, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> SetActiveAsync(Guid maintenanceRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> SetPassiveAsync(Guid maintenanceRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> AddDetailAsync(Guid maintenanceRef, Guid shipRef, MaintenanceDetailCreateModel model, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> UpdateDetailAsync(Guid detailRef, Guid shipRef, MaintenanceDetailCreateModel model, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> SetDetailActiveAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> SetDetailPassiveAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<MaintenanceResultModel> RemoveDetailAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default);
}