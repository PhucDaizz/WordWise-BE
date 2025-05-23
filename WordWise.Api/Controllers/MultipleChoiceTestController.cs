﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.AI;
using WordWise.Api.Models.Dto.MultipleChoiceTest;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MultipleChoiceTestController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMultipleChoiceTestRepository _multipleChoiceTestRepository;
        private readonly IMultipleChoiceTestService _multipleChoiceTestService;

        public MultipleChoiceTestController(IMapper mapper, IMultipleChoiceTestRepository multipleChoiceTestRepository, IMultipleChoiceTestService multipleChoiceTestService)
        {
            this.mapper = mapper;
            _multipleChoiceTestRepository = multipleChoiceTestRepository;
            _multipleChoiceTestService = multipleChoiceTestService;
        }

        [Authorize]
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody]CreateMultipleChoiceTest createMultipleChoiceTest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var multipleChoiceTest = mapper.Map<MultipleChoiceTest>(createMultipleChoiceTest);
            multipleChoiceTest.CreateAt = DateTime.UtcNow;
            multipleChoiceTest.UserId = userId;

            var test = await _multipleChoiceTestRepository.CreateAsync(multipleChoiceTest);
            if (test == null)
            {
                return BadRequest("You have reached the maximum limit of 5 Multiple Choice Exercise.");
            }
            return Ok(test);

        }


        [HttpGet]
        [Route("GetById/{multipleChoiceTestId:Guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid multipleChoiceTestId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = HttpContext.User.IsInRole("Admin") || HttpContext.User.IsInRole("SuperAdmin");

            try
            {
                var test = await _multipleChoiceTestService.GetByIdAsync(multipleChoiceTestId, userId, isAdmin);
                if (test == null)
                {
                    return NotFound("This MultipleChoiceTest is not existing!"); ;
                }
                var result = mapper.Map<MultipleChoiceTestDto>(test);
                
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("DeleteById/{multipleChoiceTestId:Guid}")]
        public async Task<IActionResult> Delete([FromRoute]Guid multipleChoiceTestId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var isAdmin = HttpContext.User.IsInRole("Admin") || HttpContext.User.IsInRole("SuperAdmin");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            var result = await _multipleChoiceTestService.DeleteByIdAsync(multipleChoiceTestId, userId, isAdmin);
            if (!result)
            {
                return BadRequest("You can not remove this multipleChoiceTest");
            }
            return Ok("This multipleChoiceTest has been deleted");
        }

        [HttpGet]
        [Route("GetAll/{userId}")]
        public async Task<IActionResult> GetAll([FromRoute] string userId, [FromQuery] int page = 1, [FromQuery] int itemPerPage = 5)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is required.");
            }

            var userIdQuery = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _multipleChoiceTestRepository.GetSummaryAsync(userId, userIdQuery, page, itemPerPage);

            if (result == null)
            {
                return NotFound("No data found.");
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("ToPublic/{multipleChoiceTestId:Guid}")]
        public async Task<IActionResult> ToPublic([FromRoute] Guid multipleChoiceTestId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var result = await _multipleChoiceTestRepository.SetStatusAsync(multipleChoiceTestId, userId);
            if (!result)
            {
                return BadRequest("You can not change this multipleChoiceTest");
            }
            return Ok("This multipleChoiceTest status has been changed");
        }

        [Authorize]
        [HttpPost]
        [Route("GenerateByAI")]
        public async Task<IActionResult> GenerateByAI([FromBody] GenerateMultipleChoiceTestRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var result = await _multipleChoiceTestService.GenerateByAIAsync(
                    userId,
                    request.LearningLanguage,
                    request.NativeLanguage,
                    request.Level,
                    request.Title
                );

                if(result == null)
                {
                    return BadRequest("You have reached the maximum limit of 5 Multiple Choice Exercise.");
                }

                var multipleChoiceTest = mapper.Map<MultipleChoiceTestDto>(result);

                return Ok(multipleChoiceTest);
            }
            catch (InvalidOperationException ex)
            {
                // Phân biệt loại lỗi dựa trên thông báo
                if (ex.Message.Contains("API Key not found"))
                {
                    return StatusCode(404, ex.Message); // 404 - Not Found
                }
                else if (ex.Message.Contains("maximum limit") || ex.Message.Contains("5 Multiple Choice Exercise"))
                {
                    return StatusCode(403, ex.Message); // 403 - Forbidden
                }
                else if (ex.Message.Contains("API key is invalid") || ex.Message.Contains("expired"))
                {
                    return StatusCode(401, ex.Message); // 401 - Unauthorized
                }
                else
                {
                    return StatusCode(429, ex.Message); // 429 - Rate limit
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try later.");
            }
        }

        [HttpGet]
        [Route("Explore")]
        public async Task<IActionResult> Explore([FromQuery]string? learningLanguage, [FromQuery] string? nativeLanguage, [FromQuery] int page = 1, [FromQuery] int itemPerPage = 20)
        {
            if (page <= 0 || itemPerPage <= 0)
            {
                return BadRequest("Page and items per page must be greater than 0.");
            }

            try
            {
                var result = await _multipleChoiceTestRepository.GetPublicAsync(learningLanguage, nativeLanguage, page, itemPerPage);

                if (result == null || result.multipleChoiceTestSummaries == null || !result.multipleChoiceTestSummaries.Any())
                {
                    return NotFound("No data found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("admin/GetAllAdmin")]
        public async Task<IActionResult> GetAllAdmin([FromQuery]Guid? multipleChoiceTestId, [FromQuery] string? learningLanguage, [FromQuery] string? nativeLanguage, [FromQuery] int page = 1, [FromQuery] int itemPerPage = 20)
        {
            try
            {
                var result = await _multipleChoiceTestRepository.GetAllAdminAsync(multipleChoiceTestId, learningLanguage, nativeLanguage, page, itemPerPage);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        


    }
}
