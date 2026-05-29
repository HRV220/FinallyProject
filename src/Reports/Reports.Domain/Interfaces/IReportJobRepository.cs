using Reports.Domain.Entities;

namespace Reports.Domain.Interfaces;

public interface IReportJobRepository
{
  Task CreateAsync(ReportJob job, CancellationToken ct = default);
  Task<ReportJob?> GetByIdAsync(Guid id, CancellationToken ct = default);
  Task UpdateAsync(ReportJob job, CancellationToken ct = default);
  Task<IReadOnlyList<ReportJob>> GetPendingAsync(int max, CancellationToken ct = default);
}
