using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WordWise.Api.Models.Dto.Room;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }


        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequestDto requestDto)
        {
            var teacherIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(teacherIdString, out Guid teacherId))
            {
                _logger.LogWarning("CreateRoom: Invalid Teacher ID format in token.");
                return Unauthorized("Invalid user identifier.");
            }

            _logger.LogInformation("Attempting to create room by Teacher ID: {TeacherId}", teacherId);
            var result = await _roomService.CreateRoomAsync(requestDto, teacherId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("CreateRoom failed for Teacher ID {TeacherId}: {Errors}", teacherId, string.Join(", ", result.Errors));
                return BadRequest(new { Errors = result.Errors });
            }

            _logger.LogInformation("Room created successfully with ID: {RoomId} and Code: {RoomCode}", result.Data!.RoomId, result.Data.RoomCode);
            return CreatedAtAction(nameof(GetRoomDetails), new { roomId = result.Data!.RoomId }, result.Data);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequestDto requestDto)
        {
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(studentIdString, out Guid studentId))
            {
                _logger.LogWarning("JoinRoom: Invalid Student ID format in token.");
                return Unauthorized("Invalid user identifier.");
            }

            _logger.LogInformation("Student ID: {StudentId} attempting to join room with code: {RoomCode}", studentId, requestDto.RoomCode);
            var result = await _roomService.JoinRoomAsync(requestDto.RoomCode, studentId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("JoinRoom failed for Student ID {StudentId}, RoomCode {RoomCode}: {Errors}", studentId, requestDto.RoomCode, string.Join(", ", result.Errors));
                return BadRequest(new { Errors = result.Errors });
            }

            _logger.LogInformation("Student ID: {StudentId} successfully joined room ID: {RoomId}", studentId, result.Data!.RoomInfo.RoomId);
            return Ok(result.Data);
        }



        [HttpGet("{roomId:Guid}")]
        public async Task<IActionResult> GetRoomDetails(Guid roomId)
        {
            _logger.LogDebug("Attempting to get details for Room ID: {RoomId}", roomId);
            var roomDetails = await _roomService.GetRoomDetailsAsync(roomId);

            if (roomDetails == null)
            {
                _logger.LogWarning("GetRoomDetails: Room ID {RoomId} not found.", roomId);
                return NotFound(new { Message = "Room not found." });
            }
            return Ok(roomDetails);
        }

        [HttpPost("{roomId:Guid}/start")]
        [Authorize]
        public async Task<IActionResult> StartRoom(Guid roomId)
        {
            var teacherIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(teacherIdString, out Guid teacherId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            _logger.LogInformation("Teacher ID: {TeacherId} attempting to start Room ID: {RoomId}", teacherId, roomId);
            var result = await _roomService.StartRoomAsync(roomId, teacherId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("StartRoom failed for Room ID {RoomId}, Teacher ID {TeacherId}: {Errors}", roomId, teacherId, string.Join(", ", result.Errors));
                return BadRequest(new { Errors = result.Errors });
            }
            _logger.LogInformation("Room ID: {RoomId} started successfully by Teacher ID: {TeacherId}", roomId, teacherId);
            return Ok(new { Message = "Room started successfully." });
        }

        [HttpPost("{roomId:guid}/next-question")]
        [Authorize]
        public async Task<IActionResult> NextQuestion(Guid roomId)
        {
            var teacherIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(teacherIdString, out Guid teacherId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            _logger.LogInformation("Teacher ID: {TeacherId} attempting to advance to next question for Room ID: {RoomId}", teacherId, roomId);
            var result = await _roomService.AdvanceToNextQuestionAsync(roomId, teacherId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("NextQuestion failed for Room ID {RoomId}, Teacher ID {TeacherId}: {Errors}", roomId, teacherId, string.Join(", ", result.Errors));
                return BadRequest(new { Errors = result.Errors });
            }
            _logger.LogInformation("Room ID: {RoomId} advanced to next question by Teacher ID: {TeacherId}", roomId, teacherId);
            return Ok(new { Message = "Advanced to next question." });
        }

        [HttpPost("{roomId:guid}/finish")]
        [Authorize]
        public async Task<IActionResult> FinishRoom(Guid roomId) 
        {
            var teacherIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(teacherIdString, out Guid teacherId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            _logger.LogInformation("Teacher ID: {TeacherId} attempting to finish Room ID: {RoomId}", teacherId, roomId);
            var result = await _roomService.FinishRoomAsync(roomId, teacherId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("FinishRoom failed for Room ID {RoomId}, Teacher ID {TeacherId}: {Errors}", roomId, teacherId, string.Join(", ", result.Errors));
                return BadRequest(new { Errors = result.Errors });
            }

            _logger.LogInformation("Room ID: {RoomId} finished by Teacher ID: {TeacherId}", roomId, teacherId);
            return Ok(new { Message = "Room finished successfully." });
        }

        [HttpGet("{roomId:guid}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(Guid roomId)
        {
            _logger.LogDebug("Attempting to get leaderboard for Room ID: {RoomId}", roomId);
            var leaderboard = await _roomService.GetLeaderboarAsync(roomId);
            if (leaderboard == null || !leaderboard.Any())
            {
                return Ok(new List<LeaderboardEntryDto>());
            }
            return Ok(leaderboard);
        }

        [HttpPost("{roomId:guid}/test-submit-answer")]
        [AllowAnonymous] 
        public async Task<IActionResult> TestSubmitAnswer(Guid roomId, [FromBody] TestSubmitAnswerRequestDtoForController request)
        {
            _logger.LogInformation("TestSubmitAnswer called for RoomId: {RoomId}, FlashcardId: {FlashcardId}", roomId, request.FlashcardId);

            // Nếu endpoint này cần xác thực người dùng (ví dụ, để biết studentId nào đang submit)
            // thì cần [Authorize] và lấy studentId từ User.Claims
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(studentIdString, out Guid studentId))
            {
                // Nếu AllowAnonymous, studentId sẽ không có. Bạn cần một cách khác để truyền studentId cho mục đích test.
                // Hoặc, nếu bạn muốn endpoint này chỉ test logic của ProcessAnswer mà không quan tâm studentId cụ thể,
                // bạn có thể cần điều chỉnh ProcessAnswerAsync để chấp nhận studentId null cho trường hợp test này (không khuyến khích cho logic chính).
                // Cách tốt hơn là endpoint này vẫn [Authorize] và bạn dùng token của student để gọi.
                // Giả sử cho mục đích test nhanh, ta hardcode một studentId nếu không có token
                if (string.IsNullOrEmpty(studentIdString))
                {
                    // CẢNH BÁO: CHỈ DÙNG CHO TEST DEV. KHÔNG BAO GIỜ HARDCODE TRONG PRODUCTION.
                    // Lấy một studentId bất kỳ từ DB hoặc hardcode một Guid đã biết.
                    // Ví dụ: studentId = Guid.Parse("your-known-student-guid-for-testing");
                    _logger.LogWarning("TestSubmitAnswer: No studentId in token, using a default or erroring. THIS IS FOR DEV TEST ONLY.");
                    // return BadRequest("Student ID is required for submitting an answer, even in test.");
                    // Tạm thời bỏ qua lỗi này để Postman không token vẫn chạy được, nhưng logic ProcessAnswer sẽ cần studentId.
                    // --> Cách tốt nhất là luôn [Authorize] và dùng token student khi test.
                }
            }


            

            var result = await _roomService.ProcessAnswerAsync(roomId, studentId, request.FlashcardId, request.AnswerText);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Errors = result.Errors });
            }
            return Ok(result.Data);
        }
    }

    // DTO riêng cho endpoint test nếu cần
    public class TestSubmitAnswerRequestDtoForController
    {
        public int FlashcardId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        // public Guid StudentIdForTest { get; set; } // Có thể thêm trường này nếu endpoint là AllowAnonymous và bạn muốn truyền studentId vào body
    }

}
