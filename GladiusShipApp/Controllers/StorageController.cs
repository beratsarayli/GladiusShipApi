using GladiusShip.Core.Service.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace GladiusShip.App.Controllers;

[Route("app/storage")]
[ApiController]
[Authorize]
public class StorageController : ControllerBase
{
    private readonly IR2StorageService _storage;

    public StorageController(IR2StorageService storage)
    {
        _storage = storage;
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> List([FromQuery] string? prefix, CancellationToken cancellationToken)
    {
        var keys = await _storage.ListKeysAsync(prefix, cancellationToken);
        return Ok(keys);
    }

    [HttpGet("{*key}")]
    public async Task<IActionResult> Get(string key, CancellationToken cancellationToken)
    {
        var stream = await _storage.GetObjectAsync(key, cancellationToken);
        if (stream == null) return NotFound();
        return File(stream, "application/octet-stream", enableRangeProcessing: true);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(CancellationToken cancellationToken)
    {
        var file = Request.Form.Files.FirstOrDefault();
        if (file == null || file.Length == 0)
            return BadRequest(new { Success = false, Message = "Dosya bulunamadý." });

        var prefix = Request.Form["prefix"].ToString();
        var ext = Path.GetExtension(file.FileName);
        var safeExt = string.IsNullOrWhiteSpace(ext) ? ".bin" : ext.ToLowerInvariant();
        var normalizedPrefix = string.IsNullOrWhiteSpace(prefix) ? "uploads" : prefix.Trim().Trim('/');
        var key = $"{normalizedPrefix}/{Guid.NewGuid()}{safeExt}";

        await using var stream = file.OpenReadStream();
        await _storage.PutObjectAsync(key, stream, file.ContentType, cancellationToken);

        return Ok(new { Success = true, Message = "Dosya yüklendi.", Key = key });
    }
}