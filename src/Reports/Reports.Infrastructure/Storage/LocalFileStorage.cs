using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reports.Application.Abstractions;

namespace Reports.Infrastructure.Storage;

/// <summary>
/// Stage 4 (минимально): сохраняем сгенерированные отчёты на локальный диск.
/// В реальном проде заменяется на S3/MinIO без изменения интерфейса.
/// </summary>
public class LocalFileStorage : IFileStorage
{
  private readonly string _root;
  private readonly string _publicBaseUrl;
  private readonly ILogger<LocalFileStorage> _logger;

  public LocalFileStorage(IConfiguration configuration, ILogger<LocalFileStorage> logger)
  {
    _root = configuration["ReportStorage:Path"] ?? Path.Combine(AppContext.BaseDirectory, "reports-data");
    _publicBaseUrl = configuration["ReportStorage:PublicBaseUrl"]?.TrimEnd('/') ?? "/api/reports/files";
    _logger = logger;

    Directory.CreateDirectory(_root);
  }

  public async Task<string> SaveAsync(string key, byte[] content, string contentType, CancellationToken ct = default)
  {
    var safe = SafeKey(key);
    var path = Path.Combine(_root, safe);
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    await File.WriteAllBytesAsync(path, content, ct);
    _logger.LogInformation("Saved report file {Key} ({Size} bytes) to {Path}", safe, content.Length, path);
    return safe;
  }

  public Task<Stream?> OpenReadAsync(string key, CancellationToken ct = default)
  {
    var path = Path.Combine(_root, SafeKey(key));
    if (!File.Exists(path))
      return Task.FromResult<Stream?>(null);
    return Task.FromResult<Stream?>(File.OpenRead(path));
  }

  public string BuildDownloadUrl(string key) => $"{_publicBaseUrl}/{SafeKey(key)}";

  private static string SafeKey(string key)
  {
    var sanitized = key.Replace("..", "_").Replace('\\', '/');
    if (Path.IsPathRooted(sanitized))
      sanitized = sanitized.TrimStart('/');
    return sanitized;
  }
}
