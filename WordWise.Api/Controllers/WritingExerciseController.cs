using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.WritingExercise;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WritingExerciseController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWritingExerciseRepository _writingExerciseRepository;
        private readonly IWritingExerciseService _writingExerciseService;

        public WritingExerciseController(IMapper mapper, IWritingExerciseRepository writingExerciseRepository, IWritingExerciseService writingExerciseService)
        {
            this.mapper = mapper;
            _writingExerciseRepository = writingExerciseRepository;
            _writingExerciseService = writingExerciseService;
        }

        [Authorize]
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateWritingExerciseDto createWritingExerciseDto)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var writingExercise = mapper.Map<WritingExercise>(createWritingExerciseDto);
            writingExercise.UserId = userId;
            writingExercise.CreateAt = DateTime.UtcNow;

            var result = await _writingExerciseRepository.CreateAsync(writingExercise);
            if(result == null)
            {
                return BadRequest("You have reached the maximum limit of 5 Writing Exercise.");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete]
        [Route("Delete/{writingExerciseId:Guid}")]
        public async Task<IActionResult> Delete([FromRoute]Guid writingExerciseId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var result = await _writingExerciseRepository.DeleteAsync(writingExerciseId, userId);
            if (!result)
            {
                return BadRequest("Cannot find this writing exercise or you don't have permission to delete it.");
            }

            return Ok("This writing exercise has been removed.");
        }

        [HttpPost]
        [Route("WriteAndGetFeedback/{writingExerciseId:Guid}")]
        public async Task<IActionResult> WriteAndGetFeedback(
            [FromRoute] Guid writingExerciseId,
            [FromBody] WriteContent writeContent)
        {
            // Kiểm tra dữ liệu đầu vào
            if (writeContent == null || string.IsNullOrEmpty(writeContent.Content))
            {
                return BadRequest(new { message = "Content is required." });
            }

            try
            {
                var writingExercise = await _writingExerciseRepository.UpdateContent(writingExerciseId, writeContent.Content);
                if (!writingExercise)
                {
                    return NotFound(new { message = $"Writing exercise with ID {writingExerciseId} not found." });
                }

                // Gọi service để lấy phản hồi từ AI
                string feedback = await _writingExerciseService.GetFeedBackFromAi(writingExerciseId);

                // Trả về phản hồi thành công
                return Ok(feedback);
            }
            catch (ArgumentException ex)
            {
                // Xử lý lỗi khi không tìm thấy writing exercise hoặc dữ liệu không hợp lệ
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Xử lý lỗi liên quan đến API key hoặc lỗi xử lý AI
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi không mong muốn khác
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("SaveWriting/{writingExerciseId:Guid}")]
        public async Task<IActionResult> SaveWriting([FromRoute]Guid writingExerciseId, [FromBody] WriteContent writeContent)
        {
            // Kiểm tra dữ liệu đầu vào
            if (writeContent == null || string.IsNullOrEmpty(writeContent.Content))
            {
                return BadRequest(new { message = "Content is required." });
            }
            try
            {
                var content =await _writingExerciseRepository.UpdateContent(writingExerciseId, writeContent.Content);
                if (!content)
                {
                    return NotFound("Writing exercise not found.");
                }
                return Ok("Save writing exercise successful");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }



        // Get feedback from AI if this writing exercise has been written
        [HttpGet]
        [Authorize]
        [Route("GetFeedback/{writingExerciseId:Guid}")]
        public async Task<IActionResult> GetFeedback([FromRoute]Guid writingExerciseId)
        {

            /*var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }*/

            try
            {
                var feedback = await _writingExerciseService.GetFeedBackFromAi(writingExerciseId);
                return Ok(feedback);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpPut]
        [Route("Update/{writingExerciseId:Guid}")]
        public async Task<IActionResult> Update([FromRoute]Guid writingExerciseId, [FromBody]UpdateWritingExercise updateWritingExercise)
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

            var result = await _writingExerciseRepository.UpdateAsync(writingExerciseId, userId, updateWritingExercise);
            return Ok("Update Writing Exercise successful");
        }

        [Authorize]
        [HttpPost]
        [Route("AutoGenerate")] 
        public async Task<IActionResult> AutoGenerate([FromBody]AutoGenerateDto autoGenerateDto)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            var writingExercise = mapper.Map<WritingExercise>(autoGenerateDto);
            writingExercise.UserId = userId;
            writingExercise.CreateAt = DateTime.UtcNow;
            try
            {
                 var writingTest = await _writingExerciseService.AutoGenerateByAi(writingExercise);
                return Ok(writingTest);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred. Please try later.");
            }

        }


    }
}
