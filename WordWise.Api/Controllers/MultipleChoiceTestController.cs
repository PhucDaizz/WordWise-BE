using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.MultipleChoiceTest;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MultipleChoiceTestController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMultipleChoiceTestRepository _multipleChoiceTestRepository;

        public MultipleChoiceTestController(IMapper mapper, IMultipleChoiceTestRepository multipleChoiceTestRepository)
        {
            this.mapper = mapper;
            _multipleChoiceTestRepository = multipleChoiceTestRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("Generate")]
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
        [Route("GetById/{MultipleChoiceTestId:Guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid MultipleChoiceTestId)
        {
            var test = await _multipleChoiceTestRepository.GetByIdAsync(MultipleChoiceTestId);
            if (test == null)
            {
                return NotFound("This MultipleChoiceTest is not existing!");
            }

            var result = mapper.Map<MultipleChoiceTestDto>(test);

            return Ok(result);
        }

        [HttpDelete]
        [Authorize]
        [Route("DeleteById/{multipleChoiceTestId:Guid}")]
        public async Task<IActionResult> Delete([FromRoute]Guid multipleChoiceTestId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            var result = await _multipleChoiceTestRepository.DeleteAsync(multipleChoiceTestId, userId);
            if (!result)
            {
                return BadRequest("You can not remove this multipleChoiceTest");
            }
            return Ok("This multipleChoiceTest has been deleted");
        }


    }
}
