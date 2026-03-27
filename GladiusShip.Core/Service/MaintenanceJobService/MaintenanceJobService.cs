using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Core.Service.Maintenance;

public class MaintenanceJobService : IMaintenanceJobService
{
    private readonly GladiusShipContext _db;

    public MaintenanceJobService(GladiusShipContext db)
    {
        _db = db;
    }

    public async Task<JobListResultModel> GetListAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MaintenanceJob
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.IsPassive == 0)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new JobListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                PersonalRef = x.PersonalRef,
                FormType = x.FormType,
                ResponsiblePersonal = x.ResponsiblePersonal,
                Description = x.Description,
                Location = x.Location,
                JobDate = x.JobDate,
                Status = x.Status,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new JobListResultModel { Success = true, Items = items };
    }

    public async Task<JobListResultModel> GetActiveJobsAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MaintenanceJob
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.IsPassive == 0 && (x.Status == 0 || x.Status == 1))
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new JobListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                PersonalRef = x.PersonalRef,
                FormType = x.FormType,
                ResponsiblePersonal = x.ResponsiblePersonal,
                Description = x.Description,
                Location = x.Location,
                JobDate = x.JobDate,
                Status = x.Status,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new JobListResultModel { Success = true, Items = items };
    }

    public async Task<JobListResultModel> GetCompletedJobsAsync(Guid shipRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MaintenanceJob
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.IsPassive == 0 && x.Status == 2)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new JobListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                PersonalRef = x.PersonalRef,
                FormType = x.FormType,
                ResponsiblePersonal = x.ResponsiblePersonal,
                Description = x.Description,
                Location = x.Location,
                JobDate = x.JobDate,
                Status = x.Status,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new JobListResultModel { Success = true, Items = items };
    }

    public async Task<JobDetailResultModel> GetDetailAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var job = await _db.MaintenanceJob
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (job == null)
            return new JobDetailResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        var items = await _db.MaintenanceJobItem
            .AsNoTracking()
            .Where(x => x.JobRef == jobRef && x.IsPassive == 0)
            .Select(x => new JobItemDetailModel
            {
                Ref = x.Ref,
                WorkItem = x.WorkItem,
                Material = x.Material,
                SerialNumber = x.SerialNumber,
                WarrantyFile = x.WarrantyFile
            })
            .ToListAsync(cancellationToken);

        var risk = await _db.MaintenanceJobRisk
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobRef == jobRef, cancellationToken);

        var cost = await _db.MaintenanceJobCost
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobRef == jobRef, cancellationToken);

        var photos = await _db.MaintenanceJobPhoto
            .AsNoTracking()
            .Where(x => x.JobRef == jobRef)
            .Select(x => new JobPhotoDetailModel
            {
                Ref = x.Ref,
                PhotoPath = x.PhotoPath,
                Category = x.Category,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        var action = await _db.MaintenanceJobAction
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobRef == jobRef, cancellationToken);

        return new JobDetailResultModel
        {
            Success = true,
            Ref = job.Ref,
            ShipRef = job.ShipRef,
            PersonalRef = job.PersonalRef,
            FormType = job.FormType,
            ResponsiblePersonal = job.ResponsiblePersonal,
            Description = job.Description,
            Location = job.Location,
            JobDate = job.JobDate,
            Status = job.Status,
            CreateDate = job.CreateDate,
            Items = items,
            Risk = risk == null ? null : new JobRiskDetailModel
            {
                OperationType = risk.OperationType,
                EngineHourBefore = risk.EngineHourBefore,
                EngineHourAfter = risk.EngineHourAfter,
                HasWaste = risk.HasWaste,
                WasteDetail = risk.WasteDetail
            },
            Cost = cost == null ? null : new JobCostDetailModel
            {
                PartCost = cost.PartCost,
                LaborCost = cost.LaborCost,
                TotalCost = cost.PartCost + cost.LaborCost,
                Currency = cost.Currency,
                InvoiceFile = cost.InvoiceFile
            },
            Photos = photos,
            Action = action == null ? null : new JobActionDetailModel
            {
                MasterComment = action.MasterComment,
                NextActionDate = action.NextActionDate,
                NextActionHour = action.NextActionHour
            }
        };
    }

    public async Task<JobResultModel> CreateDraftAsync(Guid shipRef, JobCreateModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef, cancellationToken);
        if (!shipExists)
            return new JobResultModel { Success = false, Message = "Gemi bulunamadı." };

        var job = new MaintenanceJob
        {
            Ref = Guid.NewGuid(),
            ShipRef = shipRef,
            PersonalRef = model.PersonalRef,
            FormType = model.FormType,
            ResponsiblePersonal = model.ResponsiblePersonal,
            Description = model.Description,
            Location = model.Location,
            JobDate = model.JobDate,
            Status = 0,
            IsPassive = 0,
            CreateDate = DateTime.UtcNow
        };

        await _db.MaintenanceJob.AddAsync(job, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "Taslak oluşturuldu.", JobRef = job.Ref };
    }

    public async Task<JobResultModel> UpdateAsync(Guid jobRef, Guid shipRef, JobUpdateModel model, CancellationToken cancellationToken = default)
    {
        var job = await _db.MaintenanceJob
            .FirstOrDefaultAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (job == null)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        if (job.Status == 2)
            return new JobResultModel { Success = false, Message = "Onaylanmış iş güncellenemez." };

        job.FormType = model.FormType;
        job.ResponsiblePersonal = model.ResponsiblePersonal;
        job.Description = model.Description;
        job.Location = model.Location;
        job.JobDate = model.JobDate;
        job.PersonalRef = model.PersonalRef;

        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "İş kaydı güncellendi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> SubmitForApprovalAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var job = await _db.MaintenanceJob
            .FirstOrDefaultAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (job == null)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        if (job.Status != 0)
            return new JobResultModel { Success = false, Message = "Sadece taslak işler onaya gönderilebilir." };

        job.Status = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "Onaya gönderildi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> ApproveAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var job = await _db.MaintenanceJob
            .FirstOrDefaultAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (job == null)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        if (job.Status != 1)
            return new JobResultModel { Success = false, Message = "Sadece onay bekleyen işler onaylanabilir." };

        job.Status = 2;
        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "İş onaylandı.", JobRef = jobRef };
    }

    public async Task<JobResultModel> RejectAsync(Guid jobRef, Guid shipRef, string reason, CancellationToken cancellationToken = default)
    {
        var job = await _db.MaintenanceJob
            .FirstOrDefaultAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (job == null)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        if (job.Status != 1)
            return new JobResultModel { Success = false, Message = "Sadece onay bekleyen işler reddedilebilir." };

        job.Status = 0;

        var action = await _db.MaintenanceJobAction
            .FirstOrDefaultAsync(x => x.JobRef == jobRef, cancellationToken);

        if (action == null)
        {
            await _db.MaintenanceJobAction.AddAsync(new MaintenanceJobAction
            {
                Ref = Guid.NewGuid(),
                JobRef = jobRef,
                MasterComment = reason
            }, cancellationToken);
        }
        else
        {
            action.MasterComment = reason;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "İş reddedildi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> CancelAsync(Guid jobRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var job = await _db.MaintenanceJob
            .FirstOrDefaultAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (job == null)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        if (job.Status == 2)
            return new JobResultModel { Success = false, Message = "Onaylanmış iş iptal edilemez." };

        job.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "İş iptal edildi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> AddItemAsync(Guid jobRef, Guid shipRef, JobItemModel model, CancellationToken cancellationToken = default)
    {
        var jobExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == jobRef && x.ShipRef == shipRef && x.Status != 2, cancellationToken);

        if (!jobExists)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı veya onaylanmış." };

        await _db.MaintenanceJobItem.AddAsync(new MaintenanceJobItem
        {
            Ref = Guid.NewGuid(),
            JobRef = jobRef,
            WorkItem = model.WorkItem,
            Material = model.Material,
            SerialNumber = model.SerialNumber,
            WarrantyFile = model.WarrantyFile,
            IsPassive = 0
        }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "İş kalemi eklendi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> RemoveItemAsync(Guid itemRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var item = await _db.MaintenanceJobItem.FirstOrDefaultAsync(x => x.Ref == itemRef, cancellationToken);
        if (item == null)
            return new JobResultModel { Success = false, Message = "İş kalemi bulunamadı." };

        var jobExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == item.JobRef && x.ShipRef == shipRef && x.Status != 2, cancellationToken);

        if (!jobExists)
            return new JobResultModel { Success = false, Message = "Bu kalem size ait değil veya onaylanmış." };

        item.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "İş kalemi kaldırıldı.", JobRef = item.JobRef };
    }

    public async Task<JobResultModel> SetRiskAsync(Guid jobRef, Guid shipRef, JobRiskModel model, CancellationToken cancellationToken = default)
    {
        var jobExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == jobRef && x.ShipRef == shipRef && x.Status != 2, cancellationToken);

        if (!jobExists)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı veya onaylanmış." };

        if (model.EngineHourAfter.HasValue && model.EngineHourBefore.HasValue && model.EngineHourAfter <= model.EngineHourBefore)
            return new JobResultModel { Success = false, Message = "Motor saati (Sonra) değeri (Önce) değerinden büyük olmalıdır." };

        var risk = await _db.MaintenanceJobRisk.FirstOrDefaultAsync(x => x.JobRef == jobRef, cancellationToken);

        if (risk == null)
        {
            await _db.MaintenanceJobRisk.AddAsync(new MaintenanceJobRisk
            {
                Ref = Guid.NewGuid(),
                JobRef = jobRef,
                OperationType = model.OperationType,
                EngineHourBefore = model.EngineHourBefore,
                EngineHourAfter = model.EngineHourAfter,
                HasWaste = model.HasWaste,
                WasteDetail = model.HasWaste ? model.WasteDetail : null
            }, cancellationToken);
        }
        else
        {
            risk.OperationType = model.OperationType;
            risk.EngineHourBefore = model.EngineHourBefore;
            risk.EngineHourAfter = model.EngineHourAfter;
            risk.HasWaste = model.HasWaste;
            risk.WasteDetail = model.HasWaste ? model.WasteDetail : null;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "Risk bilgisi güncellendi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> SetCostAsync(Guid jobRef, Guid shipRef, JobCostModel model, CancellationToken cancellationToken = default)
    {
        var jobExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == jobRef && x.ShipRef == shipRef && x.Status != 2, cancellationToken);

        if (!jobExists)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı veya onaylanmış." };

        var cost = await _db.MaintenanceJobCost.FirstOrDefaultAsync(x => x.JobRef == jobRef, cancellationToken);

        if (cost == null)
        {
            await _db.MaintenanceJobCost.AddAsync(new MaintenanceJobCost
            {
                Ref = Guid.NewGuid(),
                JobRef = jobRef,
                PartCost = model.PartCost,
                LaborCost = model.LaborCost,
                Currency = model.Currency,
                InvoiceFile = model.InvoiceFile
            }, cancellationToken);
        }
        else
        {
            cost.PartCost = model.PartCost;
            cost.LaborCost = model.LaborCost;
            cost.Currency = model.Currency;
            cost.InvoiceFile = model.InvoiceFile;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "Maliyet bilgisi güncellendi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> AddPhotoAsync(Guid jobRef, Guid shipRef, string photoPath, string category, CancellationToken cancellationToken = default)
    {
        var jobExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (!jobExists)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        await _db.MaintenanceJobPhoto.AddAsync(new MaintenanceJobPhoto
        {
            Ref = Guid.NewGuid(),
            JobRef = jobRef,
            PhotoPath = photoPath,
            Category = category,
            CreateDate = DateTime.UtcNow
        }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "Fotoğraf eklendi.", JobRef = jobRef };
    }

    public async Task<JobResultModel> RemovePhotoAsync(Guid photoRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var photo = await _db.MaintenanceJobPhoto.FirstOrDefaultAsync(x => x.Ref == photoRef, cancellationToken);
        if (photo == null)
            return new JobResultModel { Success = false, Message = "Fotoğraf bulunamadı." };

        var jobExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == photo.JobRef && x.ShipRef == shipRef, cancellationToken);

        if (!jobExists)
            return new JobResultModel { Success = false, Message = "Bu fotoğraf size ait değil." };

        _db.MaintenanceJobPhoto.Remove(photo);
        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "Fotoğraf silindi.", JobRef = photo.JobRef };
    }

    public async Task<JobResultModel> SetActionAsync(Guid jobRef, Guid shipRef, JobActionModel model, CancellationToken cancellationToken = default)
    {
        var jobExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == jobRef && x.ShipRef == shipRef, cancellationToken);

        if (!jobExists)
            return new JobResultModel { Success = false, Message = "İş kaydı bulunamadı." };

        var action = await _db.MaintenanceJobAction.FirstOrDefaultAsync(x => x.JobRef == jobRef, cancellationToken);

        if (action == null)
        {
            await _db.MaintenanceJobAction.AddAsync(new MaintenanceJobAction
            {
                Ref = Guid.NewGuid(),
                JobRef = jobRef,
                MasterComment = model.MasterComment,
                NextActionDate = model.NextActionDate,
                NextActionHour = model.NextActionHour
            }, cancellationToken);
        }
        else
        {
            action.MasterComment = model.MasterComment;
            action.NextActionDate = model.NextActionDate;
            action.NextActionHour = model.NextActionHour;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new JobResultModel { Success = true, Message = "Sonraki eylem planı güncellendi.", JobRef = jobRef };
    }

    public async Task<CostDocumentListResultModel> GetCostDocumentListAsync(Guid shipRef, Guid maintenanceRef, CancellationToken cancellationToken = default)
    {
        var items = await _db.MaintenanceCostDocument
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.MaintenanceRef == maintenanceRef && x.IsPassive == 0)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new CostDocumentListItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                MaintenanceRef = x.MaintenanceRef,
                Value = x.Value,
                Description = x.Description,
                FilePath = x.FilePath,
                IsPassive = x.IsPassive,
                CreateDate = x.CreateDate
            })
            .ToListAsync(cancellationToken);

        return new CostDocumentListResultModel { Success = true, Items = items };
    }

    public async Task<CostDocumentResultModel> CreateCostDocumentAsync(
     Guid shipRef,
     Guid maintenanceRef,
     CostDocumentCreateModel model,
     CancellationToken cancellationToken = default)
    {
        var maintenanceExists = await _db.MaintenanceJob
            .AnyAsync(x => x.Ref == maintenanceRef && x.ShipRef == shipRef, cancellationToken);

        if (!maintenanceExists)
            return new CostDocumentResultModel { Success = false, Message = "Bakım kaydı bulunamadı." };

        var document = new MaintenanceCostDocument
        {
            Ref = Guid.NewGuid(),
            ShipRef = shipRef,
            MaintenanceRef = maintenanceRef,
            Value = model.Value,
            Description = model.Description,
            FilePath = model.FilePath,
            IsPassive = 0,
            CreateDate = DateTime.UtcNow
        };

        await _db.MaintenanceCostDocument.AddAsync(document, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new CostDocumentResultModel
        {
            Success = true,
            Message = "Maliyet belgesi eklendi.",
            DocumentRef = document.Ref
        };
    }

    public async Task<CostDocumentResultModel> UpdateCostDocumentAsync(Guid documentRef, Guid shipRef, CostDocumentUpdateModel model, CancellationToken cancellationToken = default)
    {
        var document = await _db.MaintenanceCostDocument
            .FirstOrDefaultAsync(x => x.Ref == documentRef && x.ShipRef == shipRef, cancellationToken);

        if (document == null)
            return new CostDocumentResultModel { Success = false, Message = "Maliyet belgesi bulunamadı." };

        document.Value = model.Value;
        document.Description = model.Description;
        document.FilePath = model.FilePath;

        await _db.SaveChangesAsync(cancellationToken);

        return new CostDocumentResultModel { Success = true, Message = "Maliyet belgesi güncellendi.", DocumentRef = documentRef };
    }

    public async Task<CostDocumentResultModel> SetCostDocumentActiveAsync(Guid documentRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var document = await _db.MaintenanceCostDocument
            .FirstOrDefaultAsync(x => x.Ref == documentRef && x.ShipRef == shipRef, cancellationToken);

        if (document == null)
            return new CostDocumentResultModel { Success = false, Message = "Maliyet belgesi bulunamadı." };

        document.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new CostDocumentResultModel { Success = true, Message = "Maliyet belgesi aktife alındı.", DocumentRef = documentRef };
    }

    public async Task<CostDocumentResultModel> SetCostDocumentPassiveAsync(Guid documentRef, Guid shipRef, CancellationToken cancellationToken = default)
    {
        var document = await _db.MaintenanceCostDocument
            .FirstOrDefaultAsync(x => x.Ref == documentRef && x.ShipRef == shipRef, cancellationToken);

        if (document == null)
            return new CostDocumentResultModel { Success = false, Message = "Maliyet belgesi bulunamadı." };

        document.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new CostDocumentResultModel { Success = true, Message = "Maliyet belgesi pasife alındı.", DocumentRef = documentRef };
    }
}

#region Models

public class CostDocumentListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<CostDocumentListItemModel> Items { get; set; } = new();
}

public class CostDocumentListItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid MaintenanceRef { get; set; }
    public int Value { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int IsPassive { get; set; }
    public DateTime CreateDate { get; set; }
}

public class CostDocumentResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? DocumentRef { get; set; }
}

public class CostDocumentCreateModel
{
    public int Value { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}

public class CostDocumentUpdateModel
{
    public int Value { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}

public class JobListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<JobListItemModel> Items { get; set; } = new();
}

public class JobListItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid? PersonalRef { get; set; }
    public string FormType { get; set; } = null!;
    public string ResponsiblePersonal { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime JobDate { get; set; }
    public int Status { get; set; }
    public DateTime CreateDate { get; set; }
}

public class JobDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid? PersonalRef { get; set; }
    public string FormType { get; set; } = null!;
    public string ResponsiblePersonal { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime JobDate { get; set; }
    public int Status { get; set; }
    public DateTime CreateDate { get; set; }
    public List<JobItemDetailModel> Items { get; set; } = new();
    public JobRiskDetailModel? Risk { get; set; }
    public JobCostDetailModel? Cost { get; set; }
    public List<JobPhotoDetailModel> Photos { get; set; } = new();
    public JobActionDetailModel? Action { get; set; }
}

public class JobResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? JobRef { get; set; }
}

public class JobCreateModel
{
    public Guid? PersonalRef { get; set; }
    public string FormType { get; set; } = null!;
    public string ResponsiblePersonal { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime JobDate { get; set; }
}

public class JobUpdateModel
{
    public Guid? PersonalRef { get; set; }
    public string FormType { get; set; } = null!;
    public string ResponsiblePersonal { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime JobDate { get; set; }
}

public class JobItemModel
{
    public string WorkItem { get; set; } = null!;
    public string Material { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? WarrantyFile { get; set; }
}

public class JobItemDetailModel
{
    public Guid Ref { get; set; }
    public string WorkItem { get; set; } = null!;
    public string Material { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? WarrantyFile { get; set; }
}

public class JobRiskModel
{
    public string OperationType { get; set; } = null!;
    public int? EngineHourBefore { get; set; }
    public int? EngineHourAfter { get; set; }
    public bool HasWaste { get; set; }
    public string? WasteDetail { get; set; }
}

public class JobRiskDetailModel
{
    public string OperationType { get; set; } = null!;
    public int? EngineHourBefore { get; set; }
    public int? EngineHourAfter { get; set; }
    public bool HasWaste { get; set; }
    public string? WasteDetail { get; set; }
}

public class JobCostModel
{
    public decimal PartCost { get; set; }
    public decimal LaborCost { get; set; }
    public string Currency { get; set; } = null!;
    public string? InvoiceFile { get; set; }
}

public class JobCostDetailModel
{
    public decimal PartCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = null!;
    public string? InvoiceFile { get; set; }
}

public class JobPhotoDetailModel
{
    public Guid Ref { get; set; }
    public string PhotoPath { get; set; } = null!;
    public string Category { get; set; } = null!;
    public DateTime CreateDate { get; set; }
}

public class JobActionModel
{
    public string? MasterComment { get; set; }
    public DateTime? NextActionDate { get; set; }
    public int? NextActionHour { get; set; }
}

public class JobActionDetailModel
{
    public string? MasterComment { get; set; }
    public DateTime? NextActionDate { get; set; }
    public int? NextActionHour { get; set; }
}

#endregion