using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.ContentReport;
using WordWise.Api.Models.Enum;

namespace WordWise.Api.Repositories.Interface
{
    public interface IContentReportRepository
    {
        Task<ContentReport?> GetByIdAsync(Guid id);
        Task<GetAllContentReportDto> GetAllAsync(Guid? reportId, string? userId, ContentTypeReport? contentType, ReportStatus? status, string? sortBy, bool? isDesc, int currentPage = 1, int itemPerPage = 20);
        Task<ContentReport?> AddAsync(ContentReport contentReport);
        Task<bool> UpdateAsync(Guid id, ReportStatus status);
        Task<bool> DeleteAsync(Guid id);
    }
}
