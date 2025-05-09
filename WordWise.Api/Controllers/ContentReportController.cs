﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.ContentReport;
using WordWise.Api.Models.Enum;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentReportController : ControllerBase
    {
        private readonly IContentReportRepository _contentReportRepository;
        private readonly IMapper mapper;
        private readonly IContentReportServices _contentReportServices;

        public ContentReportController(IContentReportRepository contentReportRepository, IMapper mapper, IContentReportServices contentReportServices)
        {
            _contentReportRepository = contentReportRepository;
            this.mapper = mapper;
            _contentReportServices = contentReportServices;
        }

        [Authorize]
        [HttpPost]
        [Route("ReportContent/{ContentId}")]
        public async Task<IActionResult> ReportContent([FromRoute]string ContentId,[FromBody]CreateContentReportDto contentReport)
        {
            if (string.IsNullOrEmpty(ContentId))
            {
                return BadRequest("Content ID cannot be null or empty.");
            }
            if (contentReport == null)
            {
                return BadRequest("Content report cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }
            var report = mapper.Map<ContentReport>(contentReport);
            report.UserId = userId;
            report.ContentId = ContentId;
            report.CreateAt = DateTime.UtcNow;

            try
            {
                var result = await _contentReportRepository.AddAsync(report);
                if (result == null)
                {
                    return BadRequest("Content report already exists.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("ApproveReport/{ContentReportId:Guid}")]
        public async Task<IActionResult> ApproveReport([FromRoute]Guid ContentReportId, ReportStatus reportStatus)
        {
            try
            {
                var result = await _contentReportServices.HandleReport(ContentReportId, reportStatus);
                if (result)
                {
                    return Ok("Set status report successfully.");
                }
                else
                {
                    return NotFound("The report could not be found or updated.");
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet]
        [Route("GetAllReport")]
        public async Task<IActionResult> GetAllReport(
            [FromQuery]Guid? reportId,
            [FromQuery]string? userId,
            [FromQuery]ContentTypeReport? contentType,
            [FromQuery]ReportStatus? status,
            [FromQuery]string? sortBy,
            [FromQuery]bool? isDesc,
            [FromQuery]int currentPage = 1,
            [FromQuery]int itemPerPage = 20)
        {
            try
            {
                var result = await _contentReportRepository.GetAllAsync(
                    reportId, userId, contentType, status, sortBy, isDesc, currentPage, itemPerPage);

                if (result.ContentReports == null || !result.ContentReports.Any())
                {
                    return NotFound("No reports found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpDelete]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("DeleteReport/{ContentReportId:Guid}")]
        public async Task<IActionResult> DeleteReport([FromRoute]Guid ContentReportId)
        {
            if (ContentReportId == Guid.Empty)
            {
                return BadRequest("Content report ID cannot be empty.");
            }

            try
            {
                var result = await _contentReportRepository.DeleteAsync(ContentReportId);
                if (result)
                {
                    return Ok("Content report deleted successfully.");
                }
                else
                {
                    return NotFound("Content report not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
