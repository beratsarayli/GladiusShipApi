using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Core.Service.Insurance;

public class InsuranceService : IInsuranceService
{
    private readonly GladiusShipContext _db;

    public InsuranceService(GladiusShipContext db)
    {
        _db = db;
    }

    public async Task<InsuranceListResultModel> GetListAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.Insurance
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.IsPassive == 0)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new InsuranceListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                PersonalRef = x.PersonalRef,
                Name = x.Name,
                Description = x.Description,
                FilePath = x.FilePath,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new InsuranceListResultModel { Success = true, Items = items };
    }

    public async Task<InsuranceDetailResultModel> GetDetailAsync(Guid insuranceRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var insurance = await _db.Insurance
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == insuranceRef && x.ShipRef == shipRef, cancellationToken);

        if (insurance == null)
            return new InsuranceDetailResultModel { Success = false, Message = "Sigorta bulunamadı." };

        return new InsuranceDetailResultModel
        {
            Success = true,
            Ref = insurance.Ref,
            ShipRef = insurance.ShipRef,
            PersonalRef = insurance.PersonalRef,
            Name = insurance.Name,
            Description = insurance.Description,
            FilePath = insurance.FilePath,
            CreateDate = insurance.CreateDate,
            IsPassive = insurance.IsPassive
        };
    }

    public async Task<InsuranceResultModel> CreateAsync(Guid shipRef, InsuranceCreateModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef, cancellationToken);
        if (!shipExists)
            return new InsuranceResultModel { Success = false, Message = "Gemi bulunamadı." };

        var insurance = new Infrastructure.Entity.Insurance
        {
            Ref = Guid.NewGuid(),
            ShipRef = shipRef,
            PersonalRef = model.PersonalRef,
            Name = model.Name,
            Description = model.Description,
            FilePath = model.FilePath,
            IsPassive = 0,
            CreateDate = DateTime.UtcNow
        };

        await _db.Insurance.AddAsync(insurance, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceResultModel { Success = true, Message = "Sigorta oluşturuldu.", InsuranceRef = insurance.Ref };
    }

    public async Task<InsuranceResultModel> UpdateAsync(Guid insuranceRef, Guid shipRef, InsuranceUpdateModel model, CancellationToken cancellationToken = default)
    {
        var insurance = await _db.Insurance
            .FirstOrDefaultAsync(x => x.Ref == insuranceRef && x.ShipRef == shipRef, cancellationToken);

        if (insurance == null)
            return new InsuranceResultModel { Success = false, Message = "Sigorta bulunamadı." };

        insurance.Name = model.Name;
        insurance.Description = model.Description;
        insurance.FilePath = model.FilePath;
        insurance.PersonalRef = model.PersonalRef;

        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceResultModel { Success = true, Message = "Sigorta güncellendi.", InsuranceRef = insuranceRef };
    }

    public async Task<InsuranceResultModel> SetActiveAsync(Guid insuranceRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var insurance = await _db.Insurance
            .FirstOrDefaultAsync(x => x.Ref == insuranceRef && x.ShipRef == shipRef, cancellationToken);

        if (insurance == null)
            return new InsuranceResultModel { Success = false, Message = "Sigorta bulunamadı." };

        insurance.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceResultModel { Success = true, Message = "Sigorta aktife alındı.", InsuranceRef = insuranceRef };
    }

    public async Task<InsuranceResultModel> SetPassiveAsync(Guid insuranceRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var insurance = await _db.Insurance
            .FirstOrDefaultAsync(x => x.Ref == insuranceRef && x.ShipRef == shipRef, cancellationToken);

        if (insurance == null)
            return new InsuranceResultModel { Success = false, Message = "Sigorta bulunamadı." };

        insurance.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceResultModel { Success = true, Message = "Sigorta pasife alındı.", InsuranceRef = insuranceRef };
    }

    public async Task<ExpertiseListResultModel> GetExpertiseListAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.Expertise
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new ExpertiseListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                Description = x.Description,
                FilePath = x.FilePath,
                Value = x.Value,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new ExpertiseListResultModel { Success = true, Items = items };
    }

    public async Task<ExpertiseDetailResultModel> GetExpertiseDetailAsync(Guid expertiseRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var expertise = await _db.Expertise
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == expertiseRef && x.ShipRef == shipRef, cancellationToken);

        if (expertise == null)
            return new ExpertiseDetailResultModel { Success = false, Message = "Ekspertiz bulunamadı." };

        return new ExpertiseDetailResultModel
        {
            Success = true,
            Ref = expertise.Ref,
            ShipRef = expertise.ShipRef,
            Description = expertise.Description,
            FilePath = expertise.FilePath,
            Value = expertise.Value,
            CreateDate = expertise.CreateDate
        };
    }

    public async Task<ExpertiseResultModel> CreateExpertiseAsync(Guid shipRef, ExpertiseCreateModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef, cancellationToken);
        if (!shipExists)
            return new ExpertiseResultModel { Success = false, Message = "Gemi bulunamadı." };

        var expertise = new Infrastructure.Entity.Expertise
        {
            Ref = Guid.NewGuid(),
            ShipRef = shipRef,
            Description = model.Description,
            FilePath = model.FilePath,
            Value = model.Value,
            CreateDate = DateTime.UtcNow
        };

        await _db.Expertise.AddAsync(expertise, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new ExpertiseResultModel { Success = true, Message = "Ekspertiz eklendi.", ExpertiseRef = expertise.Ref };
    }

    public async Task<ExpertiseResultModel> UpdateExpertiseAsync(Guid expertiseRef, Guid shipRef, ExpertiseUpdateModel model, CancellationToken cancellationToken = default)
    {
        var expertise = await _db.Expertise
            .FirstOrDefaultAsync(x => x.Ref == expertiseRef && x.ShipRef == shipRef, cancellationToken);

        if (expertise == null)
            return new ExpertiseResultModel { Success = false, Message = "Ekspertiz bulunamadı." };

        expertise.Description = model.Description;
        expertise.FilePath = model.FilePath;
        expertise.Value = model.Value;

        await _db.SaveChangesAsync(cancellationToken);

        return new ExpertiseResultModel { Success = true, Message = "Ekspertiz güncellendi.", ExpertiseRef = expertiseRef };
    }

    public async Task<InsuranceDetailsListResultModel> GetInsuranceDetailsListAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.InsuranceDetails
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new InsuranceDetailsListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                Description = x.Description,
                FilePath = x.FilePath,
                Value = x.Value,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new InsuranceDetailsListResultModel { Success = true, Items = items };
    }

    public async Task<InsuranceDetailsDetailResultModel> GetInsuranceDetailsDetailAsync(Guid detailRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var detail = await _db.InsuranceDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == detailRef && x.ShipRef == shipRef, cancellationToken);

        if (detail == null)
            return new InsuranceDetailsDetailResultModel { Success = false, Message = "Sigorta detayı bulunamadı." };

        return new InsuranceDetailsDetailResultModel
        {
            Success = true,
            Ref = detail.Ref,
            ShipRef = detail.ShipRef,
            Description = detail.Description,
            FilePath = detail.FilePath,
            Value = detail.Value,
            CreateDate = detail.CreateDate
        };
    }

    public async Task<InsuranceDetailsResultModel> CreateInsuranceDetailsAsync(Guid shipRef, InsuranceDetailsCreateModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef, cancellationToken);
        if (!shipExists)
            return new InsuranceDetailsResultModel { Success = false, Message = "Gemi bulunamadı." };

        var detail = new Infrastructure.Entity.InsuranceDetails
        {
            Ref = Guid.NewGuid(),
            ShipRef = shipRef,
            Description = model.Description,
            FilePath = model.FilePath,
            Value = model.Value,
            CreateDate = DateTime.UtcNow
        };

        await _db.InsuranceDetails.AddAsync(detail, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceDetailsResultModel { Success = true, Message = "Sigorta detayı eklendi.", DetailRef = detail.Ref };
    }

    public async Task<InsuranceDetailsResultModel> UpdateInsuranceDetailsAsync(Guid detailRef, Guid shipRef, InsuranceDetailsUpdateModel model, CancellationToken cancellationToken = default)
    {
        var detail = await _db.InsuranceDetails
            .FirstOrDefaultAsync(x => x.Ref == detailRef && x.ShipRef == shipRef, cancellationToken);

        if (detail == null)
            return new InsuranceDetailsResultModel { Success = false, Message = "Sigorta detayı bulunamadı." };

        detail.Description = model.Description;
        detail.FilePath = model.FilePath;
        detail.Value = model.Value;

        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceDetailsResultModel { Success = true, Message = "Sigorta detayı güncellendi.", DetailRef = detailRef };
    }

    public async Task<InsuranceDiscountListResultModel> GetDiscountListAsync(Guid shipRef, Guid insuranceRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.InsuranceDiscount
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.InsuranceRef == insuranceRef && x.IsPassive == 0)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new InsuranceDiscountListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                InsuranceRef = x.InsuranceRef,
                Value = x.Value,
                Description = x.Description,
                FilePath = x.FilePath,
                IsPassive = x.IsPassive,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new InsuranceDiscountListResultModel { Success = true, Items = items };
    }

    public async Task<InsuranceDiscountResultModel> CreateDiscountAsync(Guid shipRef, Guid insuranceRef, InsuranceDiscountCreateModel model, CancellationToken cancellationToken = default)
    {
        var insuranceExists = await _db.Insurance
            .AnyAsync(x => x.Ref == insuranceRef && x.ShipRef == shipRef, cancellationToken);

        if (!insuranceExists)
            return new InsuranceDiscountResultModel { Success = false, Message = "Sigorta bulunamadı." };

        var discount = new Infrastructure.Entity.InsuranceDiscount
        {
            Ref = Guid.NewGuid(),
            ShipRef = shipRef,
            InsuranceRef = insuranceRef,
            Value = model.Value,
            Description = model.Description,
            FilePath = model.FilePath,
            IsPassive = 0,
            CreateDate = DateTime.UtcNow
        };

        await _db.InsuranceDiscount.AddAsync(discount, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceDiscountResultModel { Success = true, Message = "İndirim eklendi.", DiscountRef = discount.Ref };
    }

    public async Task<InsuranceDiscountResultModel> UpdateDiscountAsync(Guid discountRef, Guid shipRef, InsuranceDiscountUpdateModel model, CancellationToken cancellationToken = default)
    {
        var discount = await _db.InsuranceDiscount
            .FirstOrDefaultAsync(x => x.Ref == discountRef && x.ShipRef == shipRef, cancellationToken);

        if (discount == null)
            return new InsuranceDiscountResultModel { Success = false, Message = "İndirim bulunamadı." };

        discount.Value = model.Value;
        discount.Description = model.Description;
        discount.FilePath = model.FilePath;

        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceDiscountResultModel { Success = true, Message = "İndirim güncellendi.", DiscountRef = discountRef };
    }

    public async Task<InsuranceDiscountResultModel> SetDiscountActiveAsync(Guid discountRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var discount = await _db.InsuranceDiscount
            .FirstOrDefaultAsync(x => x.Ref == discountRef && x.ShipRef == shipRef, cancellationToken);

        if (discount == null)
            return new InsuranceDiscountResultModel { Success = false, Message = "İndirim bulunamadı." };

        discount.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceDiscountResultModel { Success = true, Message = "İndirim aktife alındı.", DiscountRef = discountRef };
    }

    public async Task<InsuranceDiscountResultModel> SetDiscountPassiveAsync(Guid discountRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var discount = await _db.InsuranceDiscount
            .FirstOrDefaultAsync(x => x.Ref == discountRef && x.ShipRef == shipRef, cancellationToken);

        if (discount == null)
            return new InsuranceDiscountResultModel { Success = false, Message = "İndirim bulunamadı." };

        discount.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new InsuranceDiscountResultModel { Success = true, Message = "İndirim pasife alındı.", DiscountRef = discountRef };
    }
}

#region Models

public class InsuranceDiscountListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<InsuranceDiscountListItemModel> Items { get; set; } = new();
}

public class InsuranceDiscountListItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid InsuranceRef { get; set; }
    public int Value { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int IsPassive { get; set; }
    public DateTime CreateDate { get; set; }
}

public class InsuranceDiscountResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? DiscountRef { get; set; }
}

public class InsuranceDiscountCreateModel
{
    public int Value { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}

public class InsuranceDiscountUpdateModel
{
    public int Value { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}
public class InsuranceDetailsListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<InsuranceDetailsListItemModel> Items { get; set; } = new();
}

public class InsuranceDetailsListItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
    public DateTime CreateDate { get; set; }
}

public class InsuranceDetailsDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
    public DateTime CreateDate { get; set; }
}

public class InsuranceDetailsResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? DetailRef { get; set; }
}

public class InsuranceDetailsCreateModel
{
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
}

public class InsuranceDetailsUpdateModel
{
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
}

public class ExpertiseListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<ExpertiseListItemModel> Items { get; set; } = new();
}

public class ExpertiseListItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
    public DateTime CreateDate { get; set; }
}

public class ExpertiseDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
    public DateTime CreateDate { get; set; }
}

public class ExpertiseResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? ExpertiseRef { get; set; }
}

public class ExpertiseCreateModel
{
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
}

public class ExpertiseUpdateModel
{
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int Value { get; set; }
}

public class InsuranceListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<InsuranceListItemModel> Items { get; set; } = new();
}

public class InsuranceListItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}

public class InsuranceDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}

public class InsuranceResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? InsuranceRef { get; set; }
}

public class InsuranceCreateModel
{
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}

public class InsuranceUpdateModel
{
    public Guid? PersonalRef { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}

#endregion