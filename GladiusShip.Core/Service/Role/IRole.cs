namespace GladiusShip.Core.Service.Role;

public interface IRoleService
{
    Task<RoleListResultModel> GetListAsync(CancellationToken cancellationToken = default);
    Task<RoleDetailResultModel> GetDetailAsync(Guid roleRef, CancellationToken cancellationToken = default);
    Task<RoleResultModel> CreateAsync(RoleCreateModel model, CancellationToken cancellationToken = default);
    Task<RoleResultModel> UpdateAsync(Guid roleRef, RoleUpdateModel model, CancellationToken cancellationToken = default);
    Task<RoleResultModel> SetActiveAsync(Guid roleRef, CancellationToken cancellationToken = default);
    Task<RoleResultModel> SetPassiveAsync(Guid roleRef, CancellationToken cancellationToken = default);
    Task<RoleResultModel> UpdatePermissionsAsync(Guid roleRef, List<string> permissions, CancellationToken cancellationToken = default);
}