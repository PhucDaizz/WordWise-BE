using Microsoft.AspNetCore.Mvc;
using WordWise.Api.Data;
using WordWise.Api.Models.Enum;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class ContentReportServices : IContentReportServices
    {
        private readonly WordWiseDbContext _dbContext;
        private readonly IContentReportRepository _contentReportRepository;

        public ContentReportServices(WordWiseDbContext dbContext ,IContentReportRepository contentReportRepository)
        {
            _dbContext = dbContext;
            _contentReportRepository = contentReportRepository;
        }

        public async Task<bool> HandleReport(Guid ContentReportId, ReportStatus reportStatus)
        {
            if (ContentReportId == Guid.Empty)
            {
                throw new ArgumentException("ContentReportId cannot be empty.");
            }
            if (!Enum.IsDefined(typeof(ReportStatus), reportStatus))
            {
                throw new ArgumentException("Invalid report status.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (reportStatus == ReportStatus.Rejected || reportStatus == ReportStatus.Approved)
                {
                    string contentId = await _contentReportRepository.UpdateAsync(ContentReportId, reportStatus);

                    if (string.IsNullOrEmpty(contentId))
                    {
                        return false;
                    }

                    var isDelete = await _contentReportRepository.DeleteDuplicateReports(ContentReportId, contentId);
                    if (!isDelete)
                    {
                        throw new Exception("Delete duplicate reports failed.");
                    }
                }

                await transaction.CommitAsync();
                return true;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while handling the report.", ex);

            }
        }
    }
}
