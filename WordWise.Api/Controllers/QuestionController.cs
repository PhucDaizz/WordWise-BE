using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.Question;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IQuestionRepository _questionRepository;

        public QuestionController(IMapper mapper, IQuestionRepository questionRepository)
        {
            this.mapper = mapper;
            _questionRepository = questionRepository;
        }

        [HttpPost]
        [Authorize]
        [Route("Create/{multipleChoiceTestId:Guid}")]
        public async Task<IActionResult> Create([FromRoute] Guid multipleChoiceTestId, [FromBody] CreateQuestionDto createQuestionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var question = mapper.Map<Question>(createQuestionDto);
            question.MultipleChoiceTestId = multipleChoiceTestId;

            var result = await _questionRepository.CreateAsync(question, userId);
            if (result == null)
            {
                return BadRequest("You can't create question on this multiplechoicetest");
            }
            else
            {
                return Ok(mapper.Map<QuestionDto>(result));
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("Delete/{questionId:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid questionId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _questionRepository.DeleteAsync(questionId, userId);
            if (result == false)
            {
                return BadRequest("You can't delete this question");
            }
            else
            {
                return Ok("This question has been deteted!");
            }
        }

        [HttpPut]
        [Authorize]
        [Route("Update/{questionId:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid questionId, [FromBody] UpdateQuestionDto updateQuestionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var question = mapper.Map<Question>(updateQuestionDto);
            question.QuestionId = questionId;

            var result = await _questionRepository.UpdateAsync(question, userId);
            if (result == null)
            {
                return BadRequest("You can't update this question");
            }
            else
            {
                return Ok(mapper.Map<QuestionDto>(result));
            }
        }

        [HttpGet]
        [Route("GetAll/{multipleChoiceTestId:Guid}")]
        public async Task<IActionResult> GetAll([FromRoute]Guid multipleChoiceTestId)
        {
            var result = await _questionRepository.GetAllAsync(multipleChoiceTestId);
            if (result == null)
            {
                return NotFound("No question found or Empty");
            }
            else
            {
                return Ok(mapper.Map<List<QuestionDto>>(result));
            }
        }   

    }
}
