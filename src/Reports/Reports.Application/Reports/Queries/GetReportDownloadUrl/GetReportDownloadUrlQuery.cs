using MediatR;
using Core.Domain.Common;
namespace Reports.Application.Reports.Queries.GetReportDownloadUrl;

public record GetReportDownloadUrlQuery(Guid ReportId, Guid RequestedBy) : IRequest<Result<GetReportDownloadUrlResponse>>;