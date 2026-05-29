using Microsoft.EntityFrameworkCore;
using Reports.Domain.Entities;
using Reports.Domain.Enums;
using Reports.Domain.Interfaces;

namespace Reports.Infrastructure.Persistence.Repositories;

public class ReportJobRepository : IReportJobRepository
{
  private readonly ReportsDbContext _context;

  public ReportJobRepository(ReportsDbContext context)
  {
    _context = context;
  }

  public async Task CreateAsync(ReportJob job, CancellationToken cancellationToken = default)
  {
    _context.ReportJobs.Add(job);
    await _context.SaveChangesAsync(cancellationToken);
  }

  public Task<ReportJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
    _context.ReportJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

  public async Task UpdateAsync(ReportJob job, CancellationToken cancellationToken = default)
  {
    await _context.ReportJobs
      .Where(j => j.Id == job.Id)
      .ExecuteUpdateAsync(s => s
        .SetProperty(j => j.Status, job.Status)
        .SetProperty(j => j.CompletedAt, job.CompletedAt)
        .SetProperty(j => j.FileKey, job.FileKey)
        .SetProperty(j => j.Error, job.Error)
        .SetProperty(j => j.UpdatedAt, DateTime.UtcNow),
        cancellationToken);
  }

  public async Task<IReadOnlyList<ReportJob>> GetPendingAsync(int max, CancellationToken cancellationToken = default)
  {
    return await _context.ReportJobs
      .AsNoTracking()
      .Where(j => j.Status == ReportStatus.Pending)
      .OrderBy(j => j.CreatedAt)
      .Take(max)
      .ToListAsync(cancellationToken);
  }
}
