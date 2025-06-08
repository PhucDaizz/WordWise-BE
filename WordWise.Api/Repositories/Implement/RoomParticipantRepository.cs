using Microsoft.EntityFrameworkCore;
using Models.Shared;
using System.Linq.Expressions;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class RoomParticipantRepository: IRoomParticipantRepository
    {
        private readonly WordWiseDbContext _dbContext;

        public RoomParticipantRepository(WordWiseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RoomParticipant roomParticipant)
        {
            await _dbContext.RoomParticipants.AddAsync(roomParticipant);
        }

        public async Task<bool> AnyAsync(Expression<Func<RoomParticipant, bool>> predicate)
        {
            return await _dbContext.RoomParticipants.AnyAsync(predicate);
        }

        public Task<RoomParticipant> CreateAsync(RoomParticipant roomParticipant)
        {
            _dbContext.RoomParticipants.AddAsync(roomParticipant);
            _dbContext.SaveChangesAsync();
            return Task.FromResult(roomParticipant);
        }

        public async Task<bool> DeleteAsync(Guid roomId, Guid userId)
        {
            var roomParticipant = await _dbContext.RoomParticipants
                .FirstOrDefaultAsync(rp => rp.RoomId == roomId && rp.UserId == userId.ToString());
            if (roomParticipant == null)
            {
                return false;
            }
            _dbContext.RoomParticipants.Remove(roomParticipant);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<RoomParticipant> GetByIdAsync(Guid roomParticipantId)
        {
            return await _dbContext.RoomParticipants
                .FirstOrDefaultAsync(rp => rp.RoomParticipantId == roomParticipantId);
        }

        public async Task<IEnumerable<RoomParticipant>> GetLeaderboardForRoomAsync(Guid roomId)
        {
            return await _dbContext.RoomParticipants
                .Where(rp => rp.RoomId == roomId)
                .Include(rp => rp.User)
                .OrderByDescending(rp => rp.Score)
                .ThenBy(rp => rp.LastActivityAt) 
                .ToListAsync();
        }

        public async Task<RoomParticipant?> GetParticipantInRoomAsync(Guid roomId, Guid userId)
        {
            return await _dbContext.RoomParticipants
                .Include(rp => rp.User)
                .FirstOrDefaultAsync(rp => rp.RoomId == roomId && rp.UserId == userId.ToString());
        }

        public async Task<IEnumerable<RoomParticipant>> GetParticipantsByRoomIdAsync(Guid roomId)
        {
            return await _dbContext.RoomParticipants
                                 .Where(rp => rp.RoomId == roomId)
                                 .Include(rp => rp.User) 
                                 .ToListAsync();
        }

        public void Remove(RoomParticipant participant)
        {
            _dbContext.RoomParticipants.Remove(participant);
        }

        public void RemoveRange(IEnumerable<RoomParticipant> participants)
        {
            _dbContext.RoomParticipants.RemoveRange(participants);
        }

        public void Update(RoomParticipant roomParticipant)
        {
            _dbContext.Entry(roomParticipant).State = EntityState.Modified;
        }
    }
    
}
