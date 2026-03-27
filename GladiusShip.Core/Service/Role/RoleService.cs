using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Core.Service.Role;

public class RoleService : IRoleService
{
    private readonly GladiusShipContext _db;

    public RoleService(GladiusShipContext db)
    {
        _db = db;
    }

    public async Task<RoleListResultModel> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _db.Roles
            .AsNoTracking()
            .Where(x => x.IsPassive == 0)
            .OrderBy(x => x.Name)
            .Select(x => new RoleListItemModel
            {
                Ref = x.Ref,
                Name = x.Name,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new RoleListResultModel { Success = true, Items = items };
    }

    public async Task<RoleDetailResultModel> GetDetailAsync(Guid roleRef, CancellationToken cancellationToken = default)
    {
        var role = await _db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == roleRef, cancellationToken);

        if (role == null)
            return new RoleDetailResultModel { Success = false, Message = "Rol bulunamadı." };

        var permissions = await _db.RoleDetails
            .AsNoTracking()
            .Where(x => x.RoleRef == roleRef)
            .Select(x => x.Permission)
            .ToListAsync(cancellationToken);

        return new RoleDetailResultModel
        {
            Success = true,
            Ref = role.Ref,
            Name = role.Name,
            CreateDate = role.CreateDate,
            IsPassive = role.IsPassive,
            Permissions = permissions
        };
    }

    public async Task<RoleResultModel> CreateAsync(RoleCreateModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            return new RoleResultModel { Success = false, Message = "Rol adı boş olamaz." };

        var nameExists = await _db.Roles.AnyAsync(x => x.Name == model.Name, cancellationToken);
        if (nameExists)
            return new RoleResultModel { Success = false, Message = "Bu isimde bir rol zaten mevcut." };

        var roleRef = Guid.NewGuid();

        await _db.Roles.AddAsync(new Infrastructure.Entity.Role
        {
            Ref = roleRef,
            Name = model.Name,
            CreateDate = DateTime.UtcNow,
            IsPassive = 0
        }, cancellationToken);

        if (model.Permissions != null && model.Permissions.Count > 0)
        {
            var details = model.Permissions
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .Select(p => new RoleDetail
                {
                    Ref = Guid.NewGuid(),
                    RoleRef = roleRef,
                    Permission = p
                }).ToList();

            await _db.RoleDetails.AddRangeAsync(details, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new RoleResultModel { Success = true, Message = "Rol oluşturuldu.", RoleRef = roleRef };
    }

    public async Task<RoleResultModel> UpdateAsync(Guid roleRef, RoleUpdateModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            return new RoleResultModel { Success = false, Message = "Rol adı boş olamaz." };

        var role = await _db.Roles.FirstOrDefaultAsync(x => x.Ref == roleRef, cancellationToken);
        if (role == null)
            return new RoleResultModel { Success = false, Message = "Rol bulunamadı." };

        var nameExists = await _db.Roles.AnyAsync(x => x.Name == model.Name && x.Ref != roleRef, cancellationToken);
        if (nameExists)
            return new RoleResultModel { Success = false, Message = "Bu isimde bir rol zaten mevcut." };

        role.Name = model.Name;
        await _db.SaveChangesAsync(cancellationToken);

        return new RoleResultModel { Success = true, Message = "Rol güncellendi.", RoleRef = roleRef };
    }

    public async Task<RoleResultModel> SetActiveAsync(Guid roleRef, CancellationToken cancellationToken = default)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(x => x.Ref == roleRef, cancellationToken);
        if (role == null)
            return new RoleResultModel { Success = false, Message = "Rol bulunamadı." };

        role.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new RoleResultModel { Success = true, Message = "Rol aktife alındı.", RoleRef = roleRef };
    }

    public async Task<RoleResultModel> SetPassiveAsync(Guid roleRef, CancellationToken cancellationToken = default)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(x => x.Ref == roleRef, cancellationToken);
        if (role == null)
            return new RoleResultModel { Success = false, Message = "Rol bulunamadı." };

        role.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new RoleResultModel { Success = true, Message = "Rol pasife alındı.", RoleRef = roleRef };
    }

    public async Task<RoleResultModel> UpdatePermissionsAsync(Guid roleRef, List<string> permissions, CancellationToken cancellationToken = default)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(x => x.Ref == roleRef, cancellationToken);
        if (role == null)
            return new RoleResultModel { Success = false, Message = "Rol bulunamadı." };

        var existing = await _db.RoleDetails
            .Where(x => x.RoleRef == roleRef)
            .ToListAsync(cancellationToken);

        _db.RoleDetails.RemoveRange(existing);

        if (permissions != null && permissions.Count > 0)
        {
            var newDetails = permissions
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .Select(p => new RoleDetail
                {
                    Ref = Guid.NewGuid(),
                    RoleRef = roleRef,
                    Permission = p
                }).ToList();

            await _db.RoleDetails.AddRangeAsync(newDetails, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new RoleResultModel { Success = true, Message = "Rol izinleri güncellendi.", RoleRef = roleRef };
    }
}

#region Models

public class RoleListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<RoleListItemModel> Items { get; set; } = new();
}

public class RoleListItemModel
{
    public Guid Ref { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}

public class RoleDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class RoleResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? RoleRef { get; set; }
}

public class RoleCreateModel
{
    public string Name { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
}

public class RoleUpdateModel
{
    public string Name { get; set; } = null!;
}

#endregion