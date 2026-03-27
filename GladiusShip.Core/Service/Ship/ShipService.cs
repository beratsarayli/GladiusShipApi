using GladiusShip.Core.Service.Storage;
using GladiusShip.Infrastructure.Context;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Core.Service.Ship;

public class ShipService : IShipService
{
    private readonly GladiusShipContext _db;
    private readonly IR2StorageService _storage;

    public ShipService(GladiusShipContext db, IR2StorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    private static (byte[] bytes, string ext, string contentType) DecodePhoto(string photo)
    {
        var base64 = photo;
        var contentType = "image/jpeg";
        if (photo.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var idx = photo.IndexOf(";base64,", StringComparison.OrdinalIgnoreCase);
            if (idx > 0)
            {
                var mime = photo[5..idx].Trim().ToLowerInvariant();
                contentType = mime;
                base64 = photo[(idx + 8)..];
            }
        }
        var ext = contentType switch
        {
            "image/png" => "png",
            "image/gif" => "gif",
            "image/webp" => "webp",
            _ => "jpg"
        };
        return (Convert.FromBase64String(base64), ext, contentType);
    }

    public async Task<ShipListResultModel> GetListAsync(Guid customerRef, CancellationToken cancellationToken = default)
    {
        var ships = await _db.Ship
            .AsNoTracking()
            .Where(x => x.CustomerRef == customerRef && x.IsPassive == 0)
            .OrderBy(x => x.Name)
            .Select(x => new ShipListItemModel
            {
                Ref = x.Ref,
                CustomerRef = x.CustomerRef,
                CompanyRef = x.CompanyRef,
                Name = x.Name,
                HullId = x.HullId,
                ImoNumber = x.ImoNumber,
                Flag = x.Flag,
                RegistrationType = x.RegistrationType
            })
            .ToListAsync(cancellationToken);

        return new ShipListResultModel { Success = true, Items = ships };
    }

    public async Task<ShipDetailResultModel> GetDetailAsync(Guid shipRef, Guid customerRef, CancellationToken cancellationToken = default)
    {
        var ship = await _db.Ship.AsNoTracking().FirstOrDefaultAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (ship == null)
            return new ShipDetailResultModel { Success = false, Message = "Gemi bulunamadı." };

        var document = await _db.ShipDocument.AsNoTracking().FirstOrDefaultAsync(x => x.ShipRef == shipRef, cancellationToken);
        var machine = await _db.ShipMachine.AsNoTracking().FirstOrDefaultAsync(x => x.ShipRef == shipRef, cancellationToken);
        var registration = await _db.ShipRegistration.AsNoTracking().FirstOrDefaultAsync(x => x.ShipRef == shipRef, cancellationToken);
        var photos = await _db.ShipPhoto.AsNoTracking().Where(x => x.ShipRef == shipRef).ToListAsync(cancellationToken);

        return new ShipDetailResultModel
        {
            Success = true,
            Ref = ship.Ref,
            CustomerRef = ship.CustomerRef,
            CompanyRef = ship.CompanyRef,
            Name = ship.Name,
            HullId = ship.HullId,
            ImoNumber = ship.ImoNumber,
            Flag = ship.Flag,
            RegistrationType = ship.RegistrationType,
            Document = document == null ? null : new ShipDocumentModel
            {
                PermitValidity = document.PermitValidity,
                InsuranceExpire = document.İnsuranceExpire,
                RadioCallSign = document.RadioCallSign,
                MMSINumber = document.MMSINumber,
                CEDocumentNumber = document.CEDocumentNumber
            },
            Machine = machine == null ? null : new ShipMachineModel
            {
                MachineBrand = machine.MachineBrand,
                MachineModel = machine.MachineModel,
                MachineSerial = machine.MachineSerial,
                Power = machine.Power,
                EngineClock = machine.EngineClock
            },
            Registration = registration == null ? null : new ShipRegistrationModel
            {
                PortRef = registration.PortRef,
                RegistrationNumber = registration.RegistrationNumber,
                RegistrationSize = registration.RegistrationSize,
                RegistrationWidth = registration.RegistrationWidth,
                GrossTonilato = registration.GrossTonilato,
                ShipCreateDate = registration.ShipCreateDate
            },
            Photos = photos.Select(x => new ShipPhotoItemModel
            {
                Ref = x.Ref,
                Photo = x.Photo,
                SerialNumber = x.SerialNumber
            }).ToList()
        };
    }

    public async Task<ShipResultModel> CreateAsync(Guid customerRef, ShipCreateModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            if (model.Photos == null || model.Photos.Count < 4)
                return new ShipResultModel { Success = false, Message = "En az 4 fotoğraf eklenmelidir." };

            var imoExists = await _db.Ship.AnyAsync(x => x.ImoNumber == model.ImoNumber, cancellationToken);
            if (imoExists)
                return new ShipResultModel { Success = false, Message = "Bu IMO numarası zaten kayıtlı." };

            var shipRef = Guid.NewGuid();

            await _db.Ship.AddAsync(new Infrastructure.Entity.Ship
            {
                Ref = shipRef,
                CustomerRef = customerRef,
                CompanyRef = model.CompanyRef,
                Name = model.Name,
                HullId = model.HullId,
                ImoNumber = model.ImoNumber,
                Flag = model.Flag,
                RegistrationType = model.RegistrationType,
                IsPassive = 0
            }, cancellationToken);

            if (model.Document != null)
            {
                await _db.ShipDocument.AddAsync(new ShipDocument
                {
                    Ref = Guid.NewGuid(),
                    ShipRef = shipRef,
                    PermitValidity = model.Document.PermitValidity,
                    İnsuranceExpire = model.Document.InsuranceExpire,
                    RadioCallSign = model.Document.RadioCallSign,
                    MMSINumber = model.Document.MMSINumber,
                    CEDocumentNumber = model.Document.CEDocumentNumber
                }, cancellationToken);
            }

            if (model.Machine != null)
            {
                await _db.ShipMachine.AddAsync(new ShipMachine
                {
                    Ref = Guid.NewGuid(),
                    ShipRef = shipRef,
                    MachineBrand = model.Machine.MachineBrand,
                    MachineModel = model.Machine.MachineModel,
                    MachineSerial = model.Machine.MachineSerial,
                    Power = model.Machine.Power,
                    EngineClock = model.Machine.EngineClock
                }, cancellationToken);
            }

            if (model.Registration != null)
            {
                await _db.ShipRegistration.AddAsync(new ShipRegistration
                {
                    Ref = Guid.NewGuid(),
                    ShipRef = shipRef,
                    PortRef = model.Registration.PortRef,
                    RegistrationNumber = model.Registration.RegistrationNumber,
                    RegistrationSize = model.Registration.RegistrationSize,
                    RegistrationWidth = model.Registration.RegistrationWidth,
                    GrossTonilato = model.Registration.GrossTonilato,
                    ShipCreateDate = model.Registration.ShipCreateDate
                }, cancellationToken);
            }

            foreach (var photo in model.Photos)
            {
                if (string.IsNullOrWhiteSpace(photo.Photo))
                    continue;

                var photoRef = Guid.NewGuid();
                string key;

                // Frontend iki türlü gelebilir:
                // 1) data:image/...;base64,...  -> decode edip storage'a yükle
                // 2) ships/.../xxx.jpg          -> zaten storage key, direkt kullan
                if (photo.Photo.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var (bytes, ext, contentType) = DecodePhoto(photo.Photo);
                    key = $"ships/{shipRef}/{photoRef}.{ext}";
                    using var stream = new MemoryStream(bytes);
                    await _storage.PutObjectAsync(key, stream, contentType, cancellationToken);
                }
                else
                {
                    key = photo.Photo.Trim();
                }

                await _db.ShipPhoto.AddAsync(new ShipPhoto
                {
                    Ref = photoRef,
                    ShipRef = shipRef,
                    Photo = key,
                    SerialNumber = photo.SerialNumber
                }, cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new ShipResultModel
            {
                Success = true,
                Message = "Gemi oluşturuldu.",
                ShipRef = shipRef
            };
        }
        catch (Exception ex)
        {
            return new ShipResultModel
            {
                Success = false,
                Message = $"Create hata: {ex.Message}"
            };
        }
    }

    public async Task<ShipResultModel> UpdateAsync(Guid shipRef, Guid customerRef, ShipUpdateModel model, CancellationToken cancellationToken = default)
    {
        var ship = await _db.Ship.FirstOrDefaultAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (ship == null)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        ship.Name = model.Name;
        ship.HullId = model.HullId;
        ship.ImoNumber = model.ImoNumber;
        ship.Flag = model.Flag;
        ship.RegistrationType = model.RegistrationType;

        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Gemi güncellendi.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> SetPassiveAsync(Guid shipRef, Guid customerRef, CancellationToken cancellationToken = default)
    {
        var ship = await _db.Ship.FirstOrDefaultAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (ship == null)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        ship.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Gemi pasife alındı.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> SetActiveAsync(Guid shipRef, Guid customerRef, CancellationToken cancellationToken = default)
    {
        var ship = await _db.Ship.FirstOrDefaultAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (ship == null)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        ship.IsPassive = 0;
        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Gemi aktife alındı.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> UpdateDocumentAsync(Guid shipRef, Guid customerRef, ShipDocumentModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!shipExists)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        var document = await _db.ShipDocument.FirstOrDefaultAsync(x => x.ShipRef == shipRef, cancellationToken);

        if (document == null)
        {
            await _db.ShipDocument.AddAsync(new ShipDocument
            {
                Ref = Guid.NewGuid(),
                ShipRef = shipRef,
                PermitValidity = model.PermitValidity,
                İnsuranceExpire = model.InsuranceExpire,
                RadioCallSign = model.RadioCallSign,
                MMSINumber = model.MMSINumber,
                CEDocumentNumber = model.CEDocumentNumber
            }, cancellationToken);
        }
        else
        {
            document.PermitValidity = model.PermitValidity;
            document.İnsuranceExpire = model.InsuranceExpire;
            document.RadioCallSign = model.RadioCallSign;
            document.MMSINumber = model.MMSINumber;
            document.CEDocumentNumber = model.CEDocumentNumber;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Gemi belgesi güncellendi.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> UpdateMachineAsync(Guid shipRef, Guid customerRef, ShipMachineModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!shipExists)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        var machine = await _db.ShipMachine.FirstOrDefaultAsync(x => x.ShipRef == shipRef, cancellationToken);

        if (machine == null)
        {
            await _db.ShipMachine.AddAsync(new ShipMachine
            {
                Ref = Guid.NewGuid(),
                ShipRef = shipRef,
                MachineBrand = model.MachineBrand,
                MachineModel = model.MachineModel,
                MachineSerial = model.MachineSerial,
                Power = model.Power,
                EngineClock = model.EngineClock
            }, cancellationToken);
        }
        else
        {
            machine.MachineBrand = model.MachineBrand;
            machine.MachineModel = model.MachineModel;
            machine.MachineSerial = model.MachineSerial;
            machine.Power = model.Power;
            machine.EngineClock = model.EngineClock;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Gemi makinesi güncellendi.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> UpdateRegistrationAsync(Guid shipRef, Guid customerRef, ShipRegistrationModel model, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!shipExists)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        var registration = await _db.ShipRegistration.FirstOrDefaultAsync(x => x.ShipRef == shipRef, cancellationToken);

        if (registration == null)
        {
            await _db.ShipRegistration.AddAsync(new ShipRegistration
            {
                Ref = Guid.NewGuid(),
                ShipRef = shipRef,
                PortRef = model.PortRef,
                RegistrationNumber = model.RegistrationNumber,
                RegistrationSize = model.RegistrationSize,
                RegistrationWidth = model.RegistrationWidth,
                GrossTonilato = model.GrossTonilato,
                ShipCreateDate = model.ShipCreateDate
            }, cancellationToken);
        }
        else
        {
            registration.PortRef = model.PortRef;
            registration.RegistrationNumber = model.RegistrationNumber;
            registration.RegistrationSize = model.RegistrationSize;
            registration.RegistrationWidth = model.RegistrationWidth;
            registration.GrossTonilato = model.GrossTonilato;
            registration.ShipCreateDate = model.ShipCreateDate;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Gemi tescili güncellendi.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> AddPhotoAsync(Guid shipRef, Guid customerRef, string photo, string serialNumber, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Ship.AnyAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!exists)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        var (bytes, ext, contentType) = DecodePhoto(photo);
        var photoRef = Guid.NewGuid();
        var key = $"ships/{shipRef}/{photoRef}.{ext}";
        using var stream = new MemoryStream(bytes);
        await _storage.PutObjectAsync(key, stream, contentType, cancellationToken);
        await _db.ShipPhoto.AddAsync(new ShipPhoto
        {
            Ref = photoRef,
            ShipRef = shipRef,
            Photo = key,
            SerialNumber = serialNumber
        }, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Fotoğraf eklendi.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> RemovePhotoAsync(Guid photoRef, Guid customerRef, CancellationToken cancellationToken = default)
    {
        var photo = await _db.ShipPhoto.FirstOrDefaultAsync(x => x.Ref == photoRef, cancellationToken);
        if (photo == null)
            return new ShipResultModel { Success = false, Message = "Fotoğraf bulunamadı." };

        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == photo.ShipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!shipExists)
            return new ShipResultModel { Success = false, Message = "Bu fotoğraf size ait değil." };

        var photoCount = await _db.ShipPhoto.CountAsync(x => x.ShipRef == photo.ShipRef, cancellationToken);
        if (photoCount <= 4)
            return new ShipResultModel { Success = false, Message = "Gemide en az 4 fotoğraf bulunmalıdır." };

        if (photo.Photo.StartsWith("ships/", StringComparison.OrdinalIgnoreCase))
            await _storage.DeleteObjectAsync(photo.Photo, cancellationToken);

        _db.ShipPhoto.Remove(photo);
        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Fotoğraf silindi." };
    }

    public async Task<ShipPermissionListResultModel> GetPermissionsAsync(Guid shipRef, Guid customerRef, CancellationToken cancellationToken = default)
    {
        var canAccess = await HasAnyShipAccessAsync(shipRef, customerRef, cancellationToken);
        if (!canAccess)
            return new ShipPermissionListResultModel { Success = false, Message = "Gemi bulunamadı veya erişim yetkiniz yok." };

        var items = await _db.ShipPermission
            .AsNoTracking()
            .Where(x => x.ShipRef == shipRef && x.IsPassive == 0)
            .OrderByDescending(x => x.CreateDate)
            .Select(x => new ShipPermissionItemModel
            {
                Ref = x.Ref,
                ShipRef = x.ShipRef,
                PersonalRef = x.PersonalRef,
                Permission = x.Permission,
                CreateDate = x.CreateDate,
                IsPassive = x.IsPassive
            })
            .ToListAsync(cancellationToken);

        return new ShipPermissionListResultModel { Success = true, Items = items };
    }

    public async Task<ShipResultModel> GrantPermissionAsync(Guid shipRef, Guid customerRef, Guid personalRef, string permission, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!shipExists)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        var existing = await _db.ShipPermission
            .FirstOrDefaultAsync(x => x.ShipRef == shipRef && x.PersonalRef == personalRef && x.Permission == permission, cancellationToken);

        if (existing != null)
        {
            existing.IsPassive = 0;
            existing.CreateDate = DateTime.UtcNow;
        }
        else
        {
            await _db.ShipPermission.AddAsync(new ShipPermission
            {
                Ref = Guid.NewGuid(),
                ShipRef = shipRef,
                PersonalRef = personalRef,
                Permission = permission,
                CreateDate = DateTime.UtcNow,
                IsPassive = 0
            }, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Yetki verildi.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> RevokePermissionAsync(Guid shipRef, Guid customerRef, Guid personalRef, string permission, CancellationToken cancellationToken = default)
    {
        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == shipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!shipExists)
            return new ShipResultModel { Success = false, Message = "Gemi bulunamadı." };

        var existing = await _db.ShipPermission
            .FirstOrDefaultAsync(x => x.ShipRef == shipRef && x.PersonalRef == personalRef && x.Permission == permission, cancellationToken);

        if (existing == null)
            return new ShipResultModel { Success = false, Message = "Yetki kaydı bulunamadı." };

        existing.IsPassive = 1;
        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Yetki kaldırıldı.", ShipRef = shipRef };
    }

    public async Task<ShipResultModel> UpdatePermissionAsync(Guid permissionRef, Guid customerRef, string permission, CancellationToken cancellationToken = default)
    {
        var perm = await _db.ShipPermission.FirstOrDefaultAsync(x => x.Ref == permissionRef, cancellationToken);
        if (perm == null)
            return new ShipResultModel { Success = false, Message = "Yetki kaydı bulunamadı." };

        var shipExists = await _db.Ship.AnyAsync(x => x.Ref == perm.ShipRef && x.CustomerRef == customerRef, cancellationToken);
        if (!shipExists)
            return new ShipResultModel { Success = false, Message = "Bu yetki size ait değil." };

        perm.Permission = permission;
        await _db.SaveChangesAsync(cancellationToken);

        return new ShipResultModel { Success = true, Message = "Yetki güncellendi.", ShipRef = perm.ShipRef };
    }
    private static bool IsReadLikePermission(string? permission)
    {
        if (string.IsNullOrWhiteSpace(permission)) return false;
        var p = permission.Trim().ToLowerInvariant();
        return p == "read" || p == "write" || p == "full";
    }

    private static bool IsWriteLikePermission(string? permission)
    {
        if (string.IsNullOrWhiteSpace(permission)) return false;
        var p = permission.Trim().ToLowerInvariant();
        return p == "write" || p == "full";
    }

    private async Task<bool> HasAnyShipAccessAsync(Guid shipRef, Guid personalRef, CancellationToken ct)
    {
        var isOwner = await _db.Ship.AnyAsync(x => x.Ref == shipRef && x.CustomerRef == personalRef, ct);
        if (isOwner) return true;

        return await _db.ShipPermission.AnyAsync(x =>
            x.ShipRef == shipRef &&
            x.PersonalRef == personalRef &&
            x.IsPassive == 0 &&
            IsReadLikePermission(x.Permission), ct);
    }

    public async Task<ShipListResultModel> GetAccessibleListAsync(Guid personalRef, CancellationToken cancellationToken = default)
    {
        var ownerShipRefs = _db.Ship
            .AsNoTracking()
            .Where(x => x.CustomerRef == personalRef && x.IsPassive == 0)
            .Select(x => x.Ref);

        var permittedShipRefs = _db.ShipPermission
            .AsNoTracking()
            .Where(x => x.PersonalRef == personalRef && x.IsPassive == 0)
            .Where(x => x.Permission == "read" || x.Permission == "write" || x.Permission == "full")
            .Select(x => x.ShipRef);

        var shipRefs = await ownerShipRefs
            .Union(permittedShipRefs)
            .Distinct()
            .ToListAsync(cancellationToken);

        var ships = await _db.Ship
            .AsNoTracking()
            .Where(x => shipRefs.Contains(x.Ref) && x.IsPassive == 0)
            .OrderBy(x => x.Name)
            .Select(x => new ShipListItemModel
            {
                Ref = x.Ref,
                CustomerRef = x.CustomerRef,
                CompanyRef = x.CompanyRef,
                Name = x.Name,
                HullId = x.HullId,
                ImoNumber = x.ImoNumber,
                Flag = x.Flag,
                RegistrationType = x.RegistrationType
            })
            .ToListAsync(cancellationToken);

        return new ShipListResultModel { Success = true, Items = ships };
    }

}

#region Models


public class ShipPermissionListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<ShipPermissionItemModel> Items { get; set; } = new();
}

public class ShipPermissionItemModel
{
    public Guid Ref { get; set; }
    public Guid ShipRef { get; set; }
    public Guid PersonalRef { get; set; }
    public string Permission { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public int IsPassive { get; set; }
}
public class ShipListResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<ShipListItemModel> Items { get; set; } = new();
}

public class ShipListItemModel
{
    public Guid Ref { get; set; }
    public Guid CustomerRef { get; set; }
    public Guid? CompanyRef { get; set; }
    public string Name { get; set; } = null!;
    public string HullId { get; set; } = null!;
    public string ImoNumber { get; set; } = null!;
    public string Flag { get; set; } = null!;
    public string RegistrationType { get; set; } = null!;
}

public class ShipDetailResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid Ref { get; set; }
    public Guid CustomerRef { get; set; }
    public Guid? CompanyRef { get; set; }
    public string Name { get; set; } = null!;
    public string HullId { get; set; } = null!;
    public string ImoNumber { get; set; } = null!;
    public string Flag { get; set; } = null!;
    public string RegistrationType { get; set; } = null!;
    public ShipDocumentModel? Document { get; set; }
    public ShipMachineModel? Machine { get; set; }
    public ShipRegistrationModel? Registration { get; set; }
    public List<ShipPhotoItemModel> Photos { get; set; } = new();
}

public class ShipResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? ShipRef { get; set; }
}

public class ShipCreateModel
{
    public Guid? CompanyRef { get; set; }
    public string Name { get; set; } = null!;
    public string HullId { get; set; } = null!;
    public string ImoNumber { get; set; } = null!;
    public string Flag { get; set; } = null!;
    public string RegistrationType { get; set; } = null!;
    public ShipDocumentModel? Document { get; set; }
    public ShipMachineModel? Machine { get; set; }
    public ShipRegistrationModel? Registration { get; set; }
    public List<ShipPhotoInputModel> Photos { get; set; } = new();
}

public class ShipUpdateModel
{
    public string Name { get; set; } = null!;
    public string HullId { get; set; } = null!;
    public string ImoNumber { get; set; } = null!;
    public string Flag { get; set; } = null!;
    public string RegistrationType { get; set; } = null!;
}

public class ShipDocumentModel
{
    public DateTime PermitValidity { get; set; }
    public DateTime InsuranceExpire { get; set; }
    public string? RadioCallSign { get; set; }
    public string? MMSINumber { get; set; }
    public string? CEDocumentNumber { get; set; }
}

public class ShipMachineModel
{
    public string MachineBrand { get; set; } = null!;
    public string MachineModel { get; set; } = null!;
    public string MachineSerial { get; set; } = null!;
    public int Power { get; set; }
    public int EngineClock { get; set; }
}

public class ShipRegistrationModel
{
    public Guid PortRef { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public int RegistrationSize { get; set; }
    public int RegistrationWidth { get; set; }
    public int GrossTonilato { get; set; }
    public DateTime ShipCreateDate { get; set; }
}

public class ShipPhotoItemModel
{
    public Guid Ref { get; set; }
    public string Photo { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
}

public class ShipPhotoInputModel
{
    public string Photo { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
}

#endregion