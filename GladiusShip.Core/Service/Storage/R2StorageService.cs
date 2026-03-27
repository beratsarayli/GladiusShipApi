using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace GladiusShip.Core.Service.Storage;

public class R2StorageService : IR2StorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;

    public R2StorageService(IAmazonS3 s3, IConfiguration configuration)
    {
        _s3 = s3;
        _bucket = configuration["R2:BucketName"] ?? "";
    }

    public async Task<IReadOnlyList<string>> ListKeysAsync(string? prefix = null, CancellationToken cancellationToken = default)
    {
        var request = new ListObjectsV2Request { BucketName = _bucket };
        if (!string.IsNullOrEmpty(prefix)) request.Prefix = prefix;
        var response = await _s3.ListObjectsV2Async(request, cancellationToken);
        return response.S3Objects?.Select(x => x.Key).ToList() ?? new List<string>();
    }

    public async Task<Stream?> GetObjectAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _s3.GetObjectAsync(_bucket, key, cancellationToken);
            return response.ResponseStream;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> PutObjectAsync(string key, Stream data, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = data,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        };
        if (!string.IsNullOrEmpty(contentType)) request.ContentType = contentType;
        var response = await _s3.PutObjectAsync(request, cancellationToken);
        return response.ETag;
    }

    public async Task<bool> DeleteObjectAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3.DeleteObjectAsync(_bucket, key, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
