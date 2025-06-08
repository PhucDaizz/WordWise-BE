using Microsoft.EntityFrameworkCore;
using Models.Shared;
using System.Linq.Expressions;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Enum;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class RoomRepository : IRoomRepository
    {
        private readonly WordWiseDbContext _dbContext;

        public RoomRepository(WordWiseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Room room)
        {
            if (room.RoomId == Guid.Empty)
            {
                room.RoomId = Guid.NewGuid();
            }
            await _dbContext.Rooms.AddAsync(room);
        }

        public async Task<bool> AnyAsync(Expression<Func<Room, bool>> predicate)
        {
            return await _dbContext.Rooms.AnyAsync(predicate);
        }

        public async Task<IEnumerable<Room>> GetActiveRoomsWithNoRecentActivityAsync(TimeSpan inactivityThreshold)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(inactivityThreshold);
            return await _dbContext.Rooms
                .Where(r => r.Status == RoomStatus.Active &&
                            r.RoomParticipants.Any() &&
                            !r.RoomParticipants.Any(rp => rp.LastActivityAt > cutoffTime))
                .Include(r => r.RoomParticipants)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(Guid roomId)
        {
            return await _dbContext.Rooms.FindAsync(roomId);
        }

        public async Task<Room?> GetByRoomCodeAsync(string roomCode)
        {
            return await _dbContext.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
        }

        public async Task<Room?> GetByRoomCodeWithIncludesAsync(string roomCode, params Expression<Func<Room, object>>[] includeProperties)
        {
            IQueryable<Room> query = _dbContext.Rooms;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
        }

        public async Task<IEnumerable<Room>> GetFinishedRoomsOlderThanAsync(TimeSpan retentionPeriod)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(retentionPeriod);
            return await _dbContext.Rooms
                .Where(r => r.Status == RoomStatus.Finished &&
                            r.EndTime != null &&
                            r.EndTime < cutoffTime)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Room> Data, int TotalRecords)> GetPagedRoomsAsync(int pageNumber, int pageSize, Expression<Func<Room, bool>>? filter = null, Func<IQueryable<Room>, IOrderedQueryable<Room>>? orderBy = null, string? includePropertiesString = null)
        {
            IQueryable<Room> query = _dbContext.Rooms;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includePropertiesString))
            {
                foreach (var includeProperty in includePropertiesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            int totalRecords = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                query = query.OrderByDescending(r => r.CreatedAt); // Sắp xếp mặc định
            }

            var data = await query.Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<Room?> GetRoomWithDetailsAsync(Guid roomId)
        {
            return await _dbContext.Rooms
                .Include(r => r.User)
                .Include(r => r.FlashcardSet)
                .Include(r => r.RoomParticipants)
                    .ThenInclude(rp => rp.User) 
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public void Remove(Room room)
        {
            _dbContext.Rooms.Remove(room);
        }

        public void Update(Room room)
        {
            var local = _dbContext.Rooms.Local.FirstOrDefault(x => x.RoomId == room.RoomId);
            if (local != null)
                _dbContext.Entry(local).State = EntityState.Detached;

            _dbContext.Entry(room).State = EntityState.Modified;
        }
    }

}
