using Reports.Domain.Entities;

namespace Reports.Application.Abstractions;

public interface IReportGenerator
{
  Task<GeneratedReport> GenerateAsync(ReportJob job, CancellationToken ct = default);
}

public record GeneratedReport(string FileKey, byte[] Content, string ContentType);
