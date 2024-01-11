namespace Demo.WebApi.Application.Storage;
public interface IAzureStorageService : IScopedService
{
    Task<(string AbsoluteUri, string BlobPath)> UploadAsync(Stream file, string partitionPath, string? fileName = null);
    Task DownloadAsync(string path, string localDownloadPath);
    Task<byte[]> DownloadAsStreamAsync(string path);
}
