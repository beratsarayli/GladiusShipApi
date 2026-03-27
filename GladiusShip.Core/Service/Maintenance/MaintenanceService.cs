using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Core.Service.Maintenance;

public class MaintenanceService : IMaintenanceService
{
    private readonly GladiusShipContext _db;

    public MaintenanceService(GladiusShipContext db)
    {
        _db = db;
    }

    public async Task<MaintenanceListResultModel> GetListAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.Maintenance
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.IsPassive == 0)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new MaintenanceListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                PersonalRef = x.PersonalRef,
                Name = x.Name,
                Description = x.Description,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new MaintenanceListResultModel { Success = true, Items = items };
    }

    public async Task<MaintenanceDetailResultModel> GetDetailAsync(Guid maintenanceRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var maintenance = await _db.Maintenance
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == maintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (maintenance == null)
            return new MaintenanceDetailResultModel { Success = false, Message = "Bakım bulunamadı." };

        var details = await _db.MaintenanceDetails
            .AsNoTracking()
            .Where(x => x.MaintenanceRef == maintenanceRef)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new MaintenanceDetailItemModel
            {
                Ref = x.Ref,
                Header = x.Header,
                Description = x.Description,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new MaintenanceDetailResultModel
        {
            Success = true,
            Ref = maintenance.Ref,
            ShipRef = maintenance.ShipRef,
            PersonalRef = maintenance.PersonalRef,
            Name = maintenance.Name,
            Description = maintenance.Description,
            CreateDate = maintenance.CreateDate,
            IsPassive = maintenance.IsPassive,
            Details = details
        };
    }

    public async Task<MaintenanceResultModel> CreateAsync(Guid shipRef, MaintenanceCreateModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef, cancellationToken);
        if (!shipExists)
            return new MaintenanceResultModel { Success = false, Message = "Gemi bulunamadı." };

        var maintenance = new Infrastructure.Entity.Maintenance
        {
            Ref = Guid.NewGuid(),
            ShipRef = shipRef,
            PersonalRef = model.PersonalRef,
            Name = model.Name,
            Description = model.Description,
            IsPassive = 0,
            CreateDate = DateTime.UtcNow
        };

        await _db.Maintenance.AddAsync(maintenance, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım oluşturuldu.", MaintenanceRef = maintenance.Ref };
    }

    public async Task<MaintenanceResultModel> UpdateAsync(Guid maintenanceRef, Guid shipRef, MaintenanceUpdateModel model, CancellationToken cancellationToken = default)
    {
        var maintenance = await _db.Maintenance
            .FirstOrDefaultAsync(x => x.Ref == maintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (maintenance == null)
            return new MaintenanceResultModel { Success = false, Message = "Bakım bulunamadı." };

        maintenance.Name = model.Name;
        maintenance.Description = model.Description;
        maintenance.PersonalRef = model.PersonalRef;

        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım güncellendi.", MaintenanceRef = maintenanceRef };
    }

    public async Task<MaintenanceResultModel> SetActiveAsync(Guid maintenanceRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var maintenance = await _db.Maintenance
            .FirstOrDefaultAsync(x => x.Ref == maintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (maintenance == null)
            return new MaintenanceResultModel { Success = false, Message = "Bakım bulunamadı." };

        maintenance.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım aktife alındı.", MaintenanceRef = maintenanceRef };
    }

    public async Task<MaintenanceResultModel> SetPassiveAsync(Guid maintenanceRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var maintenance = await _db.Maintenance
            .FirstOrDefaultAsync(x => x.Ref == maintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (maintenance == null)
            return new MaintenanceResultModel { Success = false, Message = "Bakım bulunamadı." };

        maintenance.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım pasife alındı.", MaintenanceRef = maintenanceRef };
    }

    public async Task<MaintenanceResultModel> AddDetailAsync(Guid maintenanceRef, Guid shipRef, MaintenanceDetailCreateModel model, CancellationToken cancellationToken = default)
    {
        var maintenanceExists = await _db.Maintenance
            .AnyAsync(x => x.Ref == maintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (!maintenanceExists)
            return new MaintenanceResultModel { Success = false, Message = "Bakım bulunamadı." };

        var detail = new MaintenanceDetails
        {
            Ref = Guid.NewGuid(),
            MaintenanceRef = maintenanceRef,
            Header = model.Header,
            Description = model.Description,
            IsPassive = 0,
            CreateDate = DateTime.UtcNow
        };

        await _db.MaintenanceDetails.AddAsync(detail, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım detayı eklendi.", MaintenanceRef = maintenanceRef };
    }

    public async Task<MaintenanceResultModel> UpdateDetailAsync(Guid detailRef, Guid shipRef, MaintenanceDetailCreateModel model, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MaintenanceDetails
            .FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);

        if (detail == null)
            return new MaintenanceResultModel { Success = false, Message = "Bakım detayı bulunamadı." };

        var maintenanceExists = await _db.Maintenance
            .AnyAsync(x => x.Ref == detail.MaintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (!maintenanceExists)
            return new MaintenanceResultModel { Success = false, Message = "Bu detay size ait değil." };

        detail.Header = model.Header;
        detail.Description = model.Description;

        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım detayı güncellendi.", MaintenanceRef = detail.MaintenanceRef };
    }

    public async Task<MaintenanceResultModel> SetDetailActiveAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MaintenanceDetails
            .FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);

        if (detail == null)
            return new MaintenanceResultModel { Success = false, Message = "Bakım detayı bulunamadı." };

        var maintenanceExists = await _db.Maintenance
            .AnyAsync(x => x.Ref == detail.MaintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (!maintenanceExists)
            return new MaintenanceResultModel { Success = false, Message = "Bu detay size ait değil." };

        detail.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım detayı aktife alındı.", MaintenanceRef = detail.MaintenanceRef };
    }

    public async Task<MaintenanceResultModel> SetDetailPassiveAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MaintenanceDetails
            .FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);

        if (detail == null)
            return new MaintenanceResultModel { Success = false, Message = "Bakım detayı bulunamadı." };

        var maintenanceExists = await _db.Maintenance
            .AnyAsync(x => x.Ref == detail.MaintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (!maintenanceExists)
            return new MaintenanceResultModel { Success = false, Message = "Bu detay size ait değil." };

        detail.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım detayı pasife alındı.", MaintenanceRef = detail.MaintenanceRef };
    }

    public async Task<MaintenanceResultModel> RemoveDetailAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var detail = await _db.MaintenanceDetails
            .FirstOrDefaultAsync(x => x.Ref == detailRef, cancellationToken);

        if (detail == null)
            return new MaintenanceResultModel { Success = false, Message = "Bakım detayı bulunamadı." };

        var maintenanceExists = await _db.Maintenance
            .AnyAsync(x => x.Ref == detail.MaintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (!maintenanceExists)
            return new MaintenanceResultModel { Success = false, Message = "Bu detay size ait değil." };

        _db.MaintenanceDetails.Remove(detail);
        await _db.SaveChangesAsync(cancellationToken);

        return new MaintenanceResultModel { Success = true, Message = "Bakım detayı silindi.", MaintenanceRef = detail.MaintenanceRef };
    }
}

#region Models

public class MaintenanceListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<MaintenanceListItemModel> Items { get; set; } = new();
}

public class MaintenanceListItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}

public class MaintenanceDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
    public List<MaintenanceDetailItemModel> Details { get; set; } = new();
}

public class MaintenanceDetailItemModel
{
    public Guid Ref { get; set; }
    public string Header { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}

public class MaintenanceResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? MaintenanceRef { get; set; }
}

public class MaintenanceCreateModel
{
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class MaintenanceUpdateModel
{
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class MaintenanceDetailCreateModel
{
    public string Header { get; set; } = null!;
    public string Description { get; set; } = null!;
}

#endregion