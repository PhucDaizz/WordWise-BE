using WordWise.Api.Common.Results;
using WordWise.Api.Models.Dto.Room;

namespace WordWise.Api.Services.Interface
{
    public interface IRoomService
    {
        Task<ServiceResult<RoomDto>> CreateRoomAsync(CreateRoomRequestDto request, Guid teacherId);
        Task<ServiceResult<JoinRoomResponseDto>> JoinRoomAsync(string roomCode, Guid studentId);
        Task<RoomDetailsDto?> GetRoomDetailsAsync(Guid roomId);
        Task<ServiceResult> StartRoomAsync(Guid roomId, Guid teacherId);
        Task<ServiceResult<AnswerProcessingResultDto>> ProcessAnswerAsync(Guid roomId, Guid studentId, int flashCardIdFromClient, string answer);
        Task<ServiceResult> AdvanceToNextQuestionAsync(Guid roomId, Guid teacherId);
        Task<ServiceResult> FinishRoomAsync(Guid roomId, Guid teacherId, bool triggeredByNoMoreQuestions = false);
        Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboarAsync(Guid roomId);
        Task<bool> IsUserParticipantInRoomAsync(Guid roomId, Guid userId);
        Task<UserLeftRoomDetailDto?> HandleUserDisconnectAsync(Guid userId, string connectionId);

    }
}
