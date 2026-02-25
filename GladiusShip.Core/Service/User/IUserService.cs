namespace GladiusShip.Core.Service.User;

public interface IUserService
{
    Task<IReadOnlyList<UserListItemModel>> GetListAsync(CancellationToken cancellationToken = default);
    Task<UserSetActiveResultModel> SetActiveAsync(Guid userRef, CancellationToken cancellationToken = default);
    Task<UserSetActiveResultModel> SetPassiveAsync(Guid userRef, CancellationToken cancellationToken = default);
    Task<UserSetActiveResultModel> AddAsync(Guid roleRef, string name, string surname, string phone, string mail, string password, string description, CancellationToken cancellationToken = default);
}