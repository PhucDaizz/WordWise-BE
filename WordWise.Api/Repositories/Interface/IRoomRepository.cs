using System.Linq.Expressions;
using WordWise.Api.Models.Domain;

namespace WordWise.Api.Repositories.Interface
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid roomId);
        Task<Room?> GetByRoomCodeAsync(string roomCode);
        Task<Room?> GetByRoomCodeWithIncludesAsync(string roomCode, params Expression<Func<Room, object>>[] includeProperties);
        Task<Room?> GetRoomWithDetailsAsync(Guid roomId); // Lấy phòng với participants, user, flashcardset
        Task<(IEnumerable<Room> Data, int TotalRecords)> GetPagedRoomsAsync(
            int pageNumber, int pageSize,
            Expression<Func<Room, bool>>? filter = null,
            Func<IQueryable<Room>, IOrderedQueryable<Room>>? orderBy = null,
            string? includePropertiesString = null);
        Task<IEnumerable<Room>> GetActiveRoomsWithNoRecentActivityAsync(TimeSpan inactivityThreshold);
        Task<IEnumerable<Room>> GetFinishedRoomsOlderThanAsync(TimeSpan retentionPeriod);
        Task<IEnumerable<Room>> GetWaitingRoomsOlderThanAsync(TimeSpan retentionPeriod);
        Task AddAsync(Room room);
        void Update(Room room); 
        void Remove(Room room);
        Task<bool> AnyAsync(Expression<Func<Room, bool>> predicate);
    }
}
