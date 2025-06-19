using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WordWise.Api.Services.Implement;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Hubs
{
    public class RoomHub: Hub
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomService> _logger;

        public RoomHub(IRoomService roomService, ILogger<RoomService> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [Authorize]
        public async Task JoinRoomGroup(string roomIdString) 
        {
            var userIdString = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(roomIdString, out Guid roomId) || !Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogWarning("Hub.JoinRoomGroup: Invalid parameters or user not authenticated. RoomId: {RoomIdString}, UserId: {UserIdString}", roomIdString, userIdString);
                // Không throw exception, chỉ không cho join group nếu không hợp lệ
                return;
            }

            // Check user is really a participant in this room 
            bool isParticipant = await _roomService.IsUserParticipantInRoomAsync(roomId, userId);

            if (isParticipant)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomIdString);
                _logger.LogInformation("User {UserId} (Connection: {ConnectionId}) successfully joined SignalR group for Room {RoomId}", userId, Context.ConnectionId, roomId);

                // Không cần gửi "UserJoined" từ đây nữa vì RoomService.JoinRoomAsync đã làm điều đó
                // khi người dùng thực sự tham gia phòng qua API.
                // Việc join group này chỉ là để kết nối socket nhận tin nhắn.
            }
            else
            {
                _logger.LogWarning("User {UserId} (Connection: {ConnectionId}) attempted to join SignalR group for Room {RoomId} but is NOT a valid participant in DB.", userId, Context.ConnectionId, roomId);
                // Có thể gửi một thông báo lỗi riêng cho client này nếu muốn:
                // await Clients.Caller.SendAsync("JoinGroupFailed", "You are not authorized to join this room's real-time channel.");
            }
        }

        [Authorize]
        public async Task LeaveRoomGroup(string roomIdString) 
        {
            var userIdString = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(roomIdString, out Guid roomId) || !Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogWarning("Hub.LeaveRoomGroup: Invalid parameters or user not authenticated for RoomId: {RoomIdString}", roomIdString);
                return;
            }

            // Xóa connection này khỏi group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomIdString);
            _logger.LogInformation("User {UserId} (Connection: {ConnectionId}) left SignalR group for Room {RoomId}", userId, Context.ConnectionId, roomId);

            // Thông báo cho những người khác trong phòng rằng user này đã rời
            // RoomService.HandleUserDisconnectAsync sẽ làm việc này một cách đầy đủ hơn
            // khi OnDisconnectedAsync được gọi. Chỉ gửi ở đây nếu muốn phản hồi ngay khi client chủ động gọi.
            // Tốt hơn là dựa vào OnDisconnectedAsync.
            //var userProfile = await _roomService.GetParticipantDetailsForSignalRAsync(roomId, userId); // Cần phương thức này
            // if (userProfile != null) {
            //     await Clients.Group(roomIdString).SendAsync("UserLeft", userProfile);
            // }
        }

        [Authorize]
        public async Task SubmitAnswer(string roomIdString, int flashcardId, string answerText)
        {
            var userIdString = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(roomIdString, out Guid roomId) || !Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogWarning("Hub.SubmitAnswer: Invalid parameters or user not authenticated. RoomId: {RoomIdString}, UserId: {UserIdString}", roomIdString, userIdString);
                await Clients.Caller.SendAsync("AnswerSubmissionError", new List<string> { "Authentication error or invalid room." });
                return;
            }

            _logger.LogInformation("User {UserId} submitting answer for Room {RoomId}, Flashcard {FlashcardId}", userId, roomId, flashcardId);

            var result = await _roomService.ProcessAnswerAsync(roomId, userId, flashcardId, answerText);

            if (result.IsSuccess && result.Data != null)
            {
                // Gửi kết quả chi tiết cho client vừa trả lời
                await Clients.Caller.SendAsync("AnswerResult", result.Data);
                _logger.LogDebug("Sent AnswerResult (with next question for caller) to User {UserId} for Room {RoomId}.", userId, roomId);

                if (result.Data.HasFinished)
                {
                    await Clients.Caller.SendAsync("StudentFinished", userId, roomId);
                    _logger.LogInformation("User {UserId} has completed all questions in Room {RoomId}.", userId, roomId);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("AnswerSubmissionError", result.Errors ?? new List<string> { "Unknown error processing answer." });
                _logger.LogWarning("SubmitAnswer failed for User {UserId}, Room {RoomId}, Flashcard {FlashcardId}: {Errors}", userId, roomId, flashcardId, string.Join(", ", result.Errors ?? new List<string>()));
            }
            // RoomService đã tự động gửi "LeaderboardUpdate" cho cả group sau khi xử lý câu trả lời.
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var userId = Context.UserIdentifier;
            _logger.LogInformation("Client connected: {ConnectionId}. UserId: {UserId}", Context.ConnectionId, userId ?? "Anonymous");
            await Clients.Caller.SendAsync("ConnectedSuccessfully", $"Welcome! Your connection ID is {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdString = Context.UserIdentifier;
            _logger.LogInformation("Client disconnected: {ConnectionId}. UserId: {UserId}", Context.ConnectionId, userIdString ?? "Anonymous");
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out Guid userId))
            {
                var userLeftDetail = await _roomService.HandleUserDisconnectAsync(userId, Context.ConnectionId);
                if (userLeftDetail != null)
                {
                    await Clients.Group(userLeftDetail.RoomId.ToString()).SendAsync("UserLeft", userLeftDetail.LeftParticipant);
                    _logger.LogInformation("User {UserId} (from LeftParticipant DTO) (Connection: {ConnectionId}) processed as disconnected. Sent UserLeft to Room {RoomId}",
                                         userLeftDetail.LeftParticipant.UserId, Context.ConnectionId, userLeftDetail.RoomId);

                    var leaderboard = await _roomService.GetLeaderboarAsync(userLeftDetail.RoomId);
                    if (leaderboard != null)
                    {
                        await Clients.Group(userLeftDetail.RoomId.ToString()).SendAsync("LeaderboardUpdate", leaderboard);
                        _logger.LogInformation("Sent LeaderboardUpdate to Room {RoomId} after user {UserId} disconnected.",
                                             userLeftDetail.RoomId, userLeftDetail.LeftParticipant.UserId);
                    }

                }
                else
                {
                    _logger.LogInformation("User {UserId} (Connection: {ConnectionId}) disconnected, but no active room participation found to notify.",
                                         userId, Context.ConnectionId);
                }
            }
            else
            {
                _logger.LogWarning("Client disconnected (Connection: {ConnectionId}) but UserIdentifier was invalid or empty.", Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
