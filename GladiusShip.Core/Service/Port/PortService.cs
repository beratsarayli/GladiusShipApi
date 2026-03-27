using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Core.Service.Port;

public class PortService : IPortService
{
    private readonly GladiusShipContext _db;

    public PortService(GladiusShipContext db)
    {
        _db = db;
    }

    public async Task<PortListResultModel> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _db.Port
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new PortListItemModel
            {
                Ref = x.Ref,
                Name = x.Name,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new PortListResultModel { Success = true, Items = items };
    }

    public async Task<PortDetailResultModel> GetDetailAsync(Guid portRef, CancellationToken cancellationToken = default)
    {
        var port = await _db.Port
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == portRef, cancellationToken);

        if (port == null)
            return new PortDetailResultModel { Success = false, Message = "Liman bulunamadı." };

        return new PortDetailResultModel
        {
            Success = true,
            Ref = port.Ref,
            Name = port.Name,
            IsPassive = port.IsPassive
        };
    }

    public async Task<PortResultModel> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new PortResultModel { Success = false, Message = "Liman adı boş olamaz." };

        var nameExists = await _db.Port.AnyAsync(x => x.Name == name, cancellationToken);
        if (nameExists)
            return new PortResultModel { Success = false, Message = "Bu isimde bir liman zaten mevcut." };

        var port = new Infrastructure.Entity.Port
        {
            Ref = Guid.NewGuid(),
            Name = name,
            IsPassive = 0
        };

        await _db.Port.AddAsync(port, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new PortResultModel { Success = true, Message = "Liman oluşturuldu.", PortRef = port.Ref };
    }

    public async Task<PortResultModel> UpdateAsync(Guid portRef, string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new PortResultModel { Success = false, Message = "Liman adı boş olamaz." };

        var port = await _db.Port.FirstOrDefaultAsync(x => x.Ref == portRef, cancellationToken);
        if (port == null)
            return new PortResultModel { Success = false, Message = "Liman bulunamadı." };

        var nameExists = await _db.Port.AnyAsync(x => x.Name == name && x.Ref != portRef, cancellationToken);
        if (nameExists)
            return new PortResultModel { Success = false, Message = "Bu isimde bir liman zaten mevcut." };

        port.Name = name;
        await _db.SaveChangesAsync(cancellationToken);

        return new PortResultModel { Success = true, Message = "Liman güncellendi.", PortRef = portRef };
    }

    public async Task<PortResultModel> SetActiveAsync(Guid portRef, CancellationToken cancellationToken = default)
    {
        var port = await _db.Port.FirstOrDefaultAsync(x => x.Ref == portRef, cancellationToken);
        if (port == null)
            return new PortResultModel { Success = false, Message = "Liman bulunamadı." };

        port.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new PortResultModel { Success = true, Message = "Liman aktife alındı.", PortRef = portRef };
    }

    public async Task<PortResultModel> SetPassiveAsync(Guid portRef, CancellationToken cancellationToken = default)
    {
        var port = await _db.Port.FirstOrDefaultAsync(x => x.Ref == portRef, cancellationToken);
        if (port == null)
            return new PortResultModel { Success = false, Message = "Liman bulunamadı." };

        port.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new PortResultModel { Success = true, Message = "Liman pasife alındı.", PortRef = portRef };
    }
}

#region Models

public class PortListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<PortListItemModel> Items { get; set; } = new();
}

public class PortListItemModel
{
    public Guid Ref { get; set; }
    public string Name { get; set; } = null!;
    public int IsPassive { get; set; }
}

public class PortDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public string Name { get; set; } = null!;
    public int IsPassive { get; set; }
}

public class PortResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? PortRef { get; set; }
}

#endregion