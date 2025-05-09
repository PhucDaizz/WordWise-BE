using Microsoft.AspNetCore.Mvc;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Services.Interface
{
    public interface IContentReportServices
    {
        Task<bool> HandleReport(Guid ContentReportId, ReportStatus reportStatus);
    }
}
