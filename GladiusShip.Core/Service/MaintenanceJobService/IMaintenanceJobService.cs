namespace GladiusShip.Core.Service.Maintenance;

public interface IMaintenanceJobService
{
    Task<JobListResultModel> GetListAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobListResultModel> GetActiveJobsAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobListResultModel> GetCompletedJobsAsync(Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobDetailResultModel> GetDetailAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobResultModel> CreateDraftAsync(Guid shipRef, JobCreateModel model, CancellationToken cancellationToken = default);
    Task<JobResultModel> UpdateAsync(Guid jobRef, Guid shipRef, JobUpdateModel model, CancellationToken cancellationToken = default);
    Task<JobResultModel> SubmitForApprovalAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobResultModel> ApproveAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobResultModel> RejectAsync(Guid jobRef, Guid shipRef, string reason, CancellationToken cancellationToken = default);
    Task<JobResultModel> CancelAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobResultModel> AddItemAsync(Guid jobRef, Guid shipRef, JobItemModel model, CancellationToken cancellationToken = default);
    Task<JobResultModel> RemoveItemAsync(Guid itemRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobResultModel> SetRiskAsync(Guid jobRef, Guid shipRef, JobRiskModel model, CancellationToken cancellationToken = default);
    Task<JobResultModel> SetCostAsync(Guid jobRef, Guid shipRef, JobCostModel model, CancellationToken cancellationToken = default);
    Task<JobResultModel> AddPhotoAsync(Guid jobRef, Guid shipRef, string photoPath, string category, CancellationToken cancellationToken = default);
    Task<JobResultModel> RemovePhotoAsync(Guid photoRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<JobResultModel> SetActionAsync(Guid jobRef, Guid shipRef, JobActionModel model, CancellationToken cancellationToken = default);

    Task<CostDocumentListResultModel> GetCostDocumentListAsync(Guid shipRef, Guid maintenanceRef, CancellationToken cancellationToken = default);
    Task<CostDocumentResultModel> CreateCostDocumentAsync(Guid shipRef, Guid maintenanceRef, CostDocumentCreateModel model, CancellationToken cancellationToken = default);
    Task<CostDocumentResultModel> UpdateCostDocumentAsync(Guid documentRef, Guid shipRef, CostDocumentUpdateModel model, CancellationToken cancellationToken = default);
    Task<CostDocumentResultModel> SetCostDocumentActiveAsync(Guid documentRef, Guid shipRef, CancellationToken cancellationToken = default);
    Task<CostDocumentResultModel> SetCostDocumentPassiveAsync(Guid documentRef, Guid shipRef, CancellationToken cancellationToken = default);
}