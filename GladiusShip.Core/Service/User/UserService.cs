using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GladiusShip.Core.Service.User;

public class UserService : IUserService
{
    private readonly GladiusShipContext _db;

    public UserService(GladiusShipContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<UserListItemModel>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Surname)
            .Select(x => new UserListItemModel
            {
                Ref = x.Ref,
                RoleRef = x.RoleRef,
                Name = x.Name,
                Surname = x.Surname,
                Mail = x.Mail,
                Phone = x.Phone,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UserSetActiveResultModel> SetActiveAsync(Guid userRef, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Users.AnyAsync(x => x.Ref == userRef, cancellationToken);
        if (!exists)
            return new UserSetActiveResultModel { Success = false, Message = "Kullanıcı bulunamadı." };

        await _db.Users
            .Where(x => x.Ref == userRef)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsPassive, 0), cancellationToken);

        return new UserSetActiveResultModel { Success = true, Message = "Kullanıcı aktif edildi." };
    }

    public async Task<UserSetActiveResultModel> SetPassiveAsync(Guid userRef, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Users.AnyAsync(x => x.Ref == userRef, cancellationToken);
        if (!exists)
            return new UserSetActiveResultModel { Success = false, Message = "Kullanıcı bulunamadı." };

        await _db.Users
            .Where(x => x.Ref == userRef)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsPassive, 1), cancellationToken);

        return new UserSetActiveResultModel { Success = true, Message = "Kullanıcı pasif edildi." };
    }

    #region Helper

    public async Task<UserSetActiveResultModel> AddAsync(Guid roleRef, string name, string surname, string phone, string mail, string password, string description, CancellationToken cancellationToken = default)
    {
        var mailExists = await _db.Users.AnyAsync(x => x.Mail == mail, cancellationToken);
        if (mailExists)
            return new UserSetActiveResultModel { Success = false, Message = "Bu mail adresi zaten kullanılıyor." };

        var user = new Infrastructure.Entity.User
        {
            Ref = Guid.NewGuid(),
            RoleRef = roleRef,
            Name = name,
            Surname = surname,
            Phone = phone,
            Mail = mail,
            Password = BCryptNet.HashPassword(password),
            Description = description,
            IsPassive = 0
        };

        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new UserSetActiveResultModel { Success = true, Message = "Kullanıcı eklendi." };
    }

    #endregion
}

#region Models

public class UserListItemModel
{
    public Guid Ref { get; set; }
    public Guid RoleRef { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Mail { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public int IsPassive { get; set; }
}

public class UserSetActiveResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

#endregion