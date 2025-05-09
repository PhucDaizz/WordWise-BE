using Microsoft.EntityFrameworkCore;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.ContentReport;
using WordWise.Api.Models.Enum;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class ContentReportRepository : IContentReportRepository
    {
        private readonly WordWiseDbContext _dbContext;

        public ContentReportRepository(WordWiseDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ContentReport?> AddAsync(ContentReport contentReport)
        {
            // Check if the content report already exists
            var existingReport = await _dbContext.ContentReports
                .FirstOrDefaultAsync(cr => cr.UserId == contentReport.UserId && cr.ContentId == contentReport.ContentId);

            if (existingReport != null)
            {
                return null;
            }

            var report = await _dbContext.ContentReports.AddAsync(contentReport);
            await _dbContext.SaveChangesAsync();
            return report.Entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var report = await _dbContext.ContentReports.FirstOrDefaultAsync(cr => cr.ContentReportId == id);
            if (report == null)
            {
                return false;
            }
            _dbContext.ContentReports.Remove(report);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDuplicateReports(Guid contentReportId, string contentId)
        {
            var reports = await _dbContext.ContentReports.Where(cr => cr.ContentId == contentId).ToListAsync();

            if (reports == null || !reports.Any())
            {
                return false;
            }

            var duplicateReports = reports
             .Where(cr => cr.ContentReportId != contentReportId)
             .ToList();

            _dbContext.ContentReports.RemoveRange(duplicateReports);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<GetAllContentReportDto> GetAllAsync(
            Guid? reportId,
            string? userId,
            ContentTypeReport? contentType,
            ReportStatus? status,
            string? sortBy,
            bool? isDesc,
            int currentPage = 1,
            int itemPerPage = 20)
        {
            if (currentPage <= 0 || itemPerPage <= 0)
            {
                throw new ArgumentException("Invalid pagination parameters.");
            }

            var query = _dbContext.ContentReports.AsQueryable();

            if (reportId.HasValue)
            {
                query = query.Where(cr => cr.ContentReportId == reportId.Value);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(cr => cr.UserId == userId);
            }

            if (contentType != null)
            {
                query = query.Where(cr => cr.ContentType == contentType);
            }

            if (status.HasValue)
            {
                query = query.Where(cr => cr.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "createdat" => isDesc == true ? query.OrderByDescending(cr => cr.CreateAt) : query.OrderBy(cr => cr.CreateAt),
                    "reason" => isDesc == true ? query.OrderByDescending(cr => cr.Reason) : query.OrderBy(cr => cr.Reason),
                    "status" => isDesc == true ? query.OrderByDescending(cr => cr.Status) : query.OrderBy(cr => cr.Status),
                    _ => query 
                };
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / itemPerPage);

            var items = await query
                .Skip((currentPage - 1) * itemPerPage)
                .Take(itemPerPage)
                .Select(cr => new ContentReportDto
                {
                    ContentReportId = cr.ContentReportId,
                    UserId = cr.UserId,
                    ContentId = cr.ContentId,
                    ContentType = cr.ContentType.ToString(),
                    Reason = cr.Reason,
                    Status = cr.Status,
                    CreateAt = cr.CreateAt
                })
                .ToListAsync();

            // Trả về kết quả phân trang
            return new GetAllContentReportDto
            {
                ContentReports = items,
                CurentPage = currentPage,
                ItemPerPage = itemPerPage,
                TotalPage = totalPages
            };
        }



        public async Task<ContentReport?> GetByIdAsync(Guid id)
        {
            var report = await _dbContext.ContentReports.FirstOrDefaultAsync(cr => cr.ContentReportId == id);
            if(report == null)
            {

               return null;
            }
            return report;
        }

        public async Task<string?> UpdateAsync(Guid id, ReportStatus status)
        {
            var report = await _dbContext.ContentReports.FirstOrDefaultAsync(cr => cr.ContentReportId == id);
            if (report != null)
            {
                report.Status = status;
                await _dbContext.SaveChangesAsync();
                return report.ContentId;
            }
            return null;
        }
    }
}
