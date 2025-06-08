using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class RoomDataCleaner : IRoomDataCleaner
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomDataCleaner> _logger;

        public RoomDataCleaner(IUnitOfWork unitOfWork, ILogger<RoomDataCleaner> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task CleanUpRoomDataAsync(Guid roomId)
        {
            try
            {
                _logger.LogInformation("Attempting to clean up data for Room ID: {RoomId}", roomId);
                // Get all participants ID in that Room
                var participantsInRoom = await _unitOfWork.RoomParticipants.GetParticipantsByRoomIdAsync(roomId);
                var participantIdsInRoom = participantsInRoom.Select(rp => rp.RoomParticipantId).ToList();

                if (participantIdsInRoom.Any())
                {
                    // Delete all StudentFlashcardAttempts of those participants
                    var attemptsToDelete = await _unitOfWork.StudentFlashcardAttempts.GetAttemptsByParticipantIdsAsync(participantIdsInRoom);
                    if (attemptsToDelete.Any())
                    {
                        _unitOfWork.StudentFlashcardAttempts.RemoveRange(attemptsToDelete);
                        _logger.LogDebug("Marked {Count} student flashcard attempts for deletion for Room ID: {RoomId}", attemptsToDelete.Count(), roomId);
                    }
                }
                // Delete all RoomParticipants of that Room
                if (participantsInRoom.Any())
                {
                    _unitOfWork.RoomParticipants.RemoveRange(participantsInRoom);
                    _logger.LogDebug("Marked {Count} room participants for deletion for Room ID: {RoomId}", participantsInRoom.Count(), roomId);
                }


                // 4. Delete Room 
                var roomToDelete = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                if (roomToDelete != null)
                {
                    _unitOfWork.Rooms.Remove(roomToDelete);
                    _logger.LogDebug("Marked room entity for deletion for Room ID: {RoomId}", roomId);
                }
                else
                {
                    _logger.LogWarning("Room entity with ID: {RoomId} not found during cleanup, possibly already deleted.", roomId);
                    return; 
                }

                var changes = await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Successfully cleaned up data for Room ID: {RoomId}. Changes saved: {ChangesCount}", roomId, changes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up data for Room ID: {RoomId}.", roomId);
            }
        }
    }
}
