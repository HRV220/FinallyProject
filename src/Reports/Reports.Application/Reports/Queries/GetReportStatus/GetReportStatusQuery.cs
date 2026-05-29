using MediatR;
using Core.Domain.Common;
namespace Reports.Application.Reports.Queries.GetReportStatus;

public record GetReportStatusQuery(Guid ReportId, Guid RequestedBy) : IRequest<Result<GetReportStatusResponse>>;