using System.Linq.Expressions;
using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IRoomParticipantRepository
    {
        Task<RoomParticipant> GetByIdAsync(Guid roomParticipantId);
        Task AddAsync(RoomParticipant roomParticipant);
        void Update(RoomParticipant roomParticipant);
        void Remove(RoomParticipant participant);
        void RemoveRange(IEnumerable<RoomParticipant> participants);
        Task<bool> DeleteAsync(Guid roomId, Guid userId);
        Task<RoomParticipant?> GetParticipantInRoomAsync(Guid roomId, Guid userId);
        Task<IEnumerable<RoomParticipant>> GetParticipantsByRoomIdAsync(Guid roomId);
        Task<IEnumerable<RoomParticipant>> GetLeaderboardForRoomAsync(Guid roomId);
        Task<bool> AnyAsync(Expression<Func<RoomParticipant, bool>> predicate);
    }
}
