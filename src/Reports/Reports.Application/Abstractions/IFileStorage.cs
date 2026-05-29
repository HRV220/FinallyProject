namespace Reports.Application.Abstractions;

public interface IFileStorage
{
  Task<string> SaveAsync(string key, byte[] content, string contentType, CancellationToken ct = default);
  Task<Stream?> OpenReadAsync(string key, CancellationToken ct = default);
  string BuildDownloadUrl(string key);
}
