namespace GladiusShip.Core.Service.Storage;

public interface IR2StorageService
{
    Task<IReadOnlyList<string>> ListKeysAsync(string? prefix = null, CancellationToken cancellationToken = default);
    Task<Stream?> GetObjectAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> PutObjectAsync(string key, Stream data, string? contentType = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteObjectAsync(string key, CancellationToken cancellationToken = default);
}
