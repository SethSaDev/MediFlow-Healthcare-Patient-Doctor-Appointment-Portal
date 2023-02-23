namespace MediFlow.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileAsync(string blobName, CancellationToken cancellationToken = default);
    Task<string> GetFileUrlAsync(string blobName, int expiryMinutes = 5);
    Task DeleteFileAsync(string blobName, CancellationToken cancellationToken = default);
}