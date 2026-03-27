namespace GladiusShip.Core.Service.Port;

public interface IPortService
{
    Task<PortListResultModel> GetListAsync(CancellationToken cancellationToken = default);
    Task<PortDetailResultModel> GetDetailAsync(Guid portRef, CancellationToken cancellationToken = default);
    Task<PortResultModel> CreateAsync(string name, CancellationToken cancellationToken = default);
    Task<PortResultModel> UpdateAsync(Guid portRef, string name, CancellationToken cancellationToken = default);
    Task<PortResultModel> SetActiveAsync(Guid portRef, CancellationToken cancellationToken = default);
    Task<PortResultModel> SetPassiveAsync(Guid portRef, CancellationToken cancellationToken = default);
}