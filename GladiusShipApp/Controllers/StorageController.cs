using GladiusShip.Core.Service.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<IReadOnlyList<string>>> List(
        [FromQuery] string? prefix,
        CancellationToken cancellationToken)
    {
        var keys = await _storage.ListKeysAsync(prefix, cancellationToken);
        return Ok(keys);
    }

    [HttpGet("{*key}")]
    public async Task<IActionResult> Get(string key, CancellationToken cancellationToken)
    {
        var stream = await _storage.GetObjectAsync(key, cancellationToken);
        if (stream == null) return NotFound();

        // Gerekirse MIME tespiti yapýlabilir; þimdilik generic
        return File(stream, "application/octet-stream", enableRangeProcessing: true);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(50_000_000)] // 50MB
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] IFormFile? file, [FromForm] string? prefix, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Success = false, Message = "Dosya bulunamadi." });

        var ext = Path.GetExtension(file.FileName);
        var safeExt = string.IsNullOrWhiteSpace(ext) ? ".bin" : ext.ToLowerInvariant();
        var normalizedPrefix = string.IsNullOrWhiteSpace(prefix) ? "uploads" : prefix.Trim().Trim('/');
        var key = $"{normalizedPrefix}/{Guid.NewGuid()}{safeExt}";

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;

        await using var stream = file.OpenReadStream();
        await _storage.PutObjectAsync(key, stream, contentType, cancellationToken);

        return Ok(new { Success = true, Message = "Dosya yuklendi.", Key = key });
    }
}