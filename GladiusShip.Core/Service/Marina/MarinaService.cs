using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Core.Service.Marina;

public class MarinaService : IMarinaService
{
    private readonly GladiusShipContext _db;

    public MarinaService(GladiusShipContext db)
    {
        _db = db;
    }

    public async Task<MarinaListResultModel> GetListAsync(Guid portRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.Marina
            .AsNoTracking()
            .Where(x => x.PortRef == portRef && x.IsPassive == 0)
            .OrderBy(x => x.Name)
            .Select(x => new MarinaListItemModel
            {
                Ref = x.Ref,
                PortRef = x.PortRef,
                Name = x.Name,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new MarinaListResultModel { Success = true, Items = items };
    }

    public async Task<MarinaDetailResultModel> GetDetailAsync(Guid marinaRef, Guid portRef, CancellationToken cancellationToken = default)
    {
        var marina = await _db.Marina
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == marinaRef && x.PortRef == portRef, cancellationToken);

        if (marina == null)
            return new MarinaDetailResultModel { Success = false, Message = "Marina bulunamadı." };

        var details = await _db.MarinaDetails
            .AsNoTracking()
            .Where(x => x.MarinaRef == marinaRef)
            .Select(x => new MarinaDetailItemModel
            {
                Ref = x.Ref,
                Description = x.Description,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new MarinaDetailResultModel
        {
            Success = true,
            Ref = marina.Ref,
            PortRef = marina.PortRef,
            Name = marina.Name,
            IsPassive = marina.IsPassive,
            Details = details
        };
    }

    public async Task<MarinaResultModel> CreateAsync(Guid portRef, MarinaCreateModel model, CancellationToken cancellationToken = default)
    {
        var portExists = await _db.Port.AnyAsync(x => x.Ref == portRef, cancellationToken);
        if (!portExists)
            return new MarinaResultModel { Success = false, Message = "Liman bulunamadı." };

        var nameExists = await _db.Marina.AnyAsync(x => x.PortRef == portRef && x.Name == model.Name, cancellationToken);
        if (nameExists)
            return new MarinaResultModel { Success = false, Message = "Bu isimde bir marina zaten mevcut." };

        var marinaRef = Guid.NewGuid();

        await _db.Marina.AddAsync(new Infrastructure.Entity.Marina
        {
            Ref = marinaRef,
            PortRef = portRef,
            Name = model.Name,
            IsPassive = 0
        }, cancellationToken);

        if (model.Details != null && model.Details.Count > 0)
        {
            var details = model.Details
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Select(d => new MarinaDetails
                {
                    Ref = Guid.NewGuid(),
                    MarinaRef = marinaRef,
                    Description = d,
                    IsPassive = 0
                }).ToList();

            await _db.MarinaDetails.AddRangeAsync(details, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Marina oluşturuldu.", MarinaRef = marinaRef };
    }

    public async Task<MarinaResultModel> UpdateAsync(Guid marinaRef, Guid portRef, MarinaUpdateModel model, CancellationToken cancellationToken = default)
    {
        var marina = await _db.Marina
            .FirstOrDefaultAsync(x => x.Ref == marinaRef && x.PortRef == portRef, cancellationToken);

        if (marina == null)
            return new MarinaResultModel { Success = false, Message = "Marina bulunamadı." };

        var nameExists = await _db.Marina.AnyAsync(x => x.PortRef == portRef && x.Name == model.Name && x.Ref != marinaRef, cancellationToken);
        if (nameExists)
            return new MarinaResultModel { Success = false, Message = "Bu isimde bir marina zaten mevcut." };

        marina.Name = model.Name;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Marina güncellendi.", MarinaRef = marinaRef };
    }

    public async Task<MarinaResultModel> SetActiveAsync(Guid marinaRef, Guid portRef, CancellationToken cancellationToken = default)
    {
        var marina = await _db.Marina.FirstOrDefaultAsync(x => x.Ref == marinaRef && x.PortRef == portRef, cancellationToken);
        if (marina == null)
            return new MarinaResultModel { Success = false, Message = "Marina bulunamadı." };

        marina.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Marina aktife alındı.", MarinaRef = marinaRef };
    }

    public async Task<MarinaResultModel> SetPassiveAsync(Guid marinaRef, Guid portRef, CancellationToken cancellationToken = default)
    {
        var marina = await _db.Marina.FirstOrDefaultAsync(x => x.Ref == marinaRef && x.PortRef == portRef, cancellationToken);
        if (marina == null)
            return new MarinaResultModel { Success = false, Message = "Marina bulunamadı." };

        marina.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Marina pasife alındı.", MarinaRef = marinaRef };
    }

    public async Task<MarinaResultModel> AddDetailAsync(Guid marinaRef, Guid portRef, string description, CancellationToken cancellationToken = default)
    {
        var marinaExists = await _db.Marina.AnyAsync(x => x.Ref == marinaRef && x.PortRef == portRef, cancellationToken);
        if (!marinaExists)
            return new MarinaResultModel { Success = false, Message = "Marina bulunamadı." };

        await _db.MarinaDetails.AddAsync(new MarinaDetails
        {
            Ref = Guid.NewGuid(),
            MarinaRef = marinaRef,
            Description = description,
            IsPassive = 0
        }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Detay eklendi.", MarinaRef = marinaRef };
    }

    public async Task<MarinaResultModel> UpdateDetailAsync(Guid detailRef, Guid portRef, string description, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MarinaDetails.FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);
        if (detail == null)
            return new MarinaResultModel { Success = false, Message = "Detay bulunamadı." };

        var marinaExists = await _db.Marina.AnyAsync(x => x.Ref == detail.MarinaRef && x.PortRef == portRef, cancellationToken);
        if (!marinaExists)
            return new MarinaResultModel { Success = false, Message = "Bu detay size ait değil." };

        detail.Description = description;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Detay güncellendi.", MarinaRef = detail.MarinaRef };
    }

    public async Task<MarinaResultModel> SetDetailActiveAsync(Guid detailRef, Guid portRef, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MarinaDetails.FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);
        if (detail == null)
            return new MarinaResultModel { Success = false, Message = "Detay bulunamadı." };

        var marinaExists = await _db.Marina.AnyAsync(x => x.Ref == detail.MarinaRef && x.PortRef == portRef, cancellationToken);
        if (!marinaExists)
            return new MarinaResultModel { Success = false, Message = "Bu detay size ait değil." };

        detail.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Detay aktife alındı.", MarinaRef = detail.MarinaRef };
    }

    public async Task<MarinaResultModel> SetDetailPassiveAsync(Guid detailRef, Guid portRef, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MarinaDetails.FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);
        if (detail == null)
            return new MarinaResultModel { Success = false, Message = "Detay bulunamadı." };

        var marinaExists = await _db.Marina.AnyAsync(x => x.Ref == detail.MarinaRef && x.PortRef == portRef, cancellationToken);
        if (!marinaExists)
            return new MarinaResultModel { Success = false, Message = "Bu detay size ait değil." };

        detail.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Detay pasife alındı.", MarinaRef = detail.MarinaRef };
    }

    public async Task<MarinaResultModel> RemoveDetailAsync(Guid detailRef, Guid portRef, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MarinaDetails.FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);
        if (detail == null)
            return new MarinaResultModel { Success = false, Message = "Detay bulunamadı." };

        var marinaExists = await _db.Marina.AnyAsync(x => x.Ref == detail.MarinaRef && x.PortRef == portRef, cancellationToken);
        if (!marinaExists)
            return new MarinaResultModel { Success = false, Message = "Bu detay size ait değil." };

        _db.MarinaDetails.Remove(detail);
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Detay silindi.", MarinaRef = detail.MarinaRef };
    }

    public async Task<MarinaResultModel> ShipEntryAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var portExists = await _db.Port.AnyAsync(x => x.Ref == portRef, cancellationToken);
        if (!portExists)
            return new MarinaResultModel { Success = false, Message = "Liman bulunamadı." };

        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef, cancellationToken);
        if (!shipExists)
            return new MarinaResultModel { Success = false, Message = "Gemi bulunamadı." };

        var hasPermission = await _db.MarinaPermission
            .AnyAsync(x => x.PortRef == portRef && x.ShipRef == shipRef && x.IsPassive == 0 && x.Status == 1, cancellationToken);

        if (!hasPermission)
            return new MarinaResultModel { Success = false, Message = "Geminin bu limana giriş yetkisi bulunmamaktadır." };

        var alreadyInPort = await _db.MarinaRoad
            .AnyAsync(x => x.PortRef == portRef && x.ShipRef == shipRef && x.Status == 1 && x.IsPassive == 0, cancellationToken);

        if (alreadyInPort)
            return new MarinaResultModel { Success = false, Message = "Gemi zaten bu limanda bulunmaktadır." };

        await _db.MarinaRoad.AddAsync(new MarinaRoad
        {
            Ref = Guid.NewGuid(),
            PortRef = portRef,
            ShipRef = shipRef,
            Status = 1,
            CreateDate = DateTime.UtcNow,
            IsPassive = 0
        }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Gemi limana giriş yaptı." };
    }

    public async Task<MarinaResultModel> ShipExitAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var road = await _db.MarinaRoad
            .FirstOrDefaultAsync(x => x.PortRef == portRef && x.ShipRef == shipRef && x.Status == 1 && x.IsPassive == 0, cancellationToken);

        if (road == null)
            return new MarinaResultModel { Success = false, Message = "Gemi bu limanda bulunamadı." };

        road.Status = 0;
        road.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Gemi limandan çıkış yaptı." };
    }

    public async Task<MarinaRoadListResultModel> GetShipRoadsAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MarinaRoad
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new MarinaRoadItemModel
            {
                Ref = x.Ref,
                PortRef = x.PortRef,
                ShipRef = x.ShipRef,
                Status = x.Status,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new MarinaRoadListResultModel { Success = true, Items = items };
    }

    public async Task<MarinaRoadListResultModel> GetPortRoadsAsync(Guid portRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MarinaRoad
            .AsNoTracking()
            .Where(x => x.PortRef == portRef)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new MarinaRoadItemModel
            {
                Ref = x.Ref,
                PortRef = x.PortRef,
                ShipRef = x.ShipRef,
                Status = x.Status,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new MarinaRoadListResultModel { Success = true, Items = items };
    }

    public async Task<MarinaResultModel> GrantPermissionAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var portExists = await _db.Port.AnyAsync(x => x.Ref == portRef, cancellationToken);
        if (!portExists)
            return new MarinaResultModel { Success = false, Message = "Liman bulunamadı." };

        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef, cancellationToken);
        if (!shipExists)
            return new MarinaResultModel { Success = false, Message = "Gemi bulunamadı." };

        var existing = await _db.MarinaPermission
            .FirstOrDefaultAsync(x => x.PortRef == portRef && x.ShipRef == shipRef, cancellationToken);

        if (existing != null)
        {
            existing.Status = 1;
            existing.IsPassive = 0;
        }
        else
        {
            await _db.MarinaPermission.AddAsync(new MarinaPermission
            {
                Ref = Guid.NewGuid(),
                PortRef = portRef,
                ShipRef = shipRef,
                Status = 1,
                CreateDate = DateTime.UtcNow,
                IsPassive = 0
            }, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Gemi için liman girişi yetkilendirildi." };
    }

    public async Task<MarinaResultModel> RevokePermissionAsync(Guid portRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var permission = await _db.MarinaPermission
            .FirstOrDefaultAsync(x => x.PortRef == portRef && x.ShipRef == shipRef, cancellationToken);

        if (permission == null)
            return new MarinaResultModel { Success = false, Message = "Yetki kaydı bulunamadı." };

        permission.Status = 0;
        permission.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Gemi için liman girişi yetkisi kaldırıldı." };
    }

    public async Task<MarinaPermissionListResultModel> GetPermissionsAsync(Guid portRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MarinaPermission
            .AsNoTracking()
            .Where(x => x.PortRef == portRef)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new MarinaPermissionItemModel
            {
                Ref = x.Ref,
                PortRef = x.PortRef,
                ShipRef = x.ShipRef,
                Status = x.Status,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new MarinaPermissionListResultModel { Success = true, Items = items };
    }

    public async Task<MarinaResultModel> AddPriceAsync(Guid marinaRef, MarinaPriceCreateModel model, CancellationToken cancellationToken = default)
    {
        var marinaExists = await _db.Marina.AnyAsync(x => x.Ref == marinaRef, cancellationToken);
        if (!marinaExists)
            return new MarinaResultModel { Success = false, Message = "Marina bulunamadı." };

        await _db.MarinaPrice.AddAsync(new MarinaPrice
        {
            Ref = Guid.NewGuid(),
            MarinaRef = marinaRef,
            ShipRef = model.ShipRef,
            CustomerRef = model.CustomerRef,
            Description = model.Description,
            Value = model.Value,
            CreateDate = DateTime.UtcNow
        }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Fiyat eklendi.", MarinaRef = marinaRef };
    }

    public async Task<MarinaPriceListResultModel> GetPricesAsync(Guid marinaRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MarinaPrice
            .AsNoTracking()
            .Where(x => x.MarinaRef == marinaRef)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new MarinaPriceItemModel
            {
                Ref = x.Ref,
                MarinaRef = x.MarinaRef,
                ShipRef = x.ShipRef,
                CustomerRef = x.CustomerRef,
                Description = x.Description,
                Value = x.Value,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new MarinaPriceListResultModel { Success = true, Items = items };
    }

    public async Task<MarinaResultModel> UpdatePricePaymentStatusAsync(
    Guid marinaRef,
    Guid priceRef,
    MarinaPricePaymentUpdateModel model,
    CancellationToken cancellationToken = default)
    {
        var price = await _db.MarinaPrice
            .FirstOrDefaultAsync(x => x.Ref == priceRef && x.MarinaRef == marinaRef, cancellationToken);

        if (price == null)
            return new MarinaResultModel { Success = false, Message = "Fiyat kaydı bulunamadı." };

        var raw = price.Description ?? string.Empty;
        var cleaned = raw.Replace("[[PAID]]", string.Empty)
                         .Replace("[[UNPAID]]", string.Empty)
                         .Trim();

        price.Description = $"{(model.IsPaid ? "[[PAID]]" : "[[UNPAID]]")} {cleaned}".Trim();
        await _db.SaveChangesAsync(cancellationToken);

        return new MarinaResultModel { Success = true, Message = "Ödeme durumu güncellendi.", MarinaRef = marinaRef };
    }
}

#region Models

public class MarinaPricePaymentUpdateModel
{
    public bool IsPaid { get; set; }
}

public class MarinaListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<MarinaListItemModel> Items { get; set; } = new();
}

public class MarinaListItemModel
{
    public Guid Ref { get; set; }
    public Guid PortRef { get; set; }
    public string Name { get; set; } = null!;
    public int IsPassive { get; set; }
}

public class MarinaDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public Guid PortRef { get; set; }
    public string Name { get; set; } = null!;
    public int IsPassive { get; set; }
    public List<MarinaDetailItemModel> Details { get; set; } = new();
}

public class MarinaDetailItemModel
{
    public Guid Ref { get; set; }
    public string Description { get; set; } = null!;
    public int IsPassive { get; set; }
}

public class MarinaResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? MarinaRef { get; set; }
}

public class MarinaCreateModel
{
    public string Name { get; set; } = null!;
    public List<string>? Details { get; set; }
}

public class MarinaUpdateModel
{
    public string Name { get; set; } = null!;
}

public class MarinaRoadListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<MarinaRoadItemModel> Items { get; set; } = new();
}

public class MarinaRoadItemModel
{
    public Guid Ref { get; set; }
    public Guid PortRef { get; set; }
    public Guid ShipRef { get; set; }
    public int Status { get; set; }
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}

public class MarinaPermissionListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<MarinaPermissionItemModel> Items { get; set; } = new();
}

public class MarinaPermissionItemModel
{
    public Guid Ref { get; set; }
    public Guid PortRef { get; set; }
    public Guid ShipRef { get; set; }
    public int Status { get; set; }
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}

public class MarinaPriceCreateModel
{
    public Guid ShipRef { get; set; }
    public Guid CustomerRef { get; set; }
    public string Description { get; set; } = null!;
    public int Value { get; set; }
}

public class MarinaPriceListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<MarinaPriceItemModel> Items { get; set; } = new();
}

public class MarinaPriceItemModel
{
    public Guid Ref { get; set; }
    public Guid MarinaRef { get; set; }
    public Guid ShipRef { get; set; }
    public Guid CustomerRef { get; set; }
    public string Description { get; set; } = null!;
    public int Value { get; set; }
    public DateTime CreateDate { get; set; }
}

#endregion