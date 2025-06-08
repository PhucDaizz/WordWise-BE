using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WordWise.Api.Data;
using WordWise.Api.Models.Domain;
using WordWise.Api.Repositories.Interface;

namespace WordWise.Api.Repositories.Implement
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WordWiseDbContext _dbContext;
        private readonly UserManager<ExtendedIdentityUser> _userManager;
        private readonly IMapper _mapper;

        public IRoomRepository Rooms { get; private set; }
        public IRoomParticipantRepository RoomParticipants { get; private set; }
        public IStudentFlashcardAttemptRepository StudentFlashcardAttempts { get; private set; }
        public IFlashcardSetRepository FlashcardSets { get; private set; }
        public IFlashCardRepository Flashcards { get; private set; }
        public IAuthRepository Auth { get; private set; } 

        public UnitOfWork(WordWiseDbContext dbContext, UserManager<ExtendedIdentityUser> userManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;

            Rooms = new RoomRepository(_dbContext);
            RoomParticipants = new RoomParticipantRepository(_dbContext);
            StudentFlashcardAttempts = new StudentFlashcardAttemptRepository(_dbContext);
            FlashcardSets = new FlashcardSetRepository(_dbContext);
            Flashcards = new FlashCardRepository(_dbContext);
            Auth = new AuthRepository(_dbContext, _userManager, _mapper);
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose(); 
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
