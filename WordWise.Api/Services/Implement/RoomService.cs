using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WordWise.Api.Common.Results;
using WordWise.Api.Data;
using WordWise.Api.Hubs;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.Room;
using WordWise.Api.Models.Enum;
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.Services.Implement
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<RoomHub> _roomHubContext;
        private readonly ILogger<RoomService> _logger;
        private readonly IMapper _mapper;
        private readonly WordWiseDbContext _dbContext;

        public RoomService(IUnitOfWork unitOfWork, IHubContext<RoomHub> roomHubContext, ILogger<RoomService> logger, IMapper mapper, WordWiseDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _roomHubContext = roomHubContext;
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<ServiceResult> AdvanceToNextQuestionAsync(Guid roomId, Guid teacherId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            var teacherIdString = teacherId.ToString();

            if (room == null) return ServiceResult.Failure("Room not found.");
            if (room.UserId != teacherIdString) return ServiceResult.Failure("Unauthorized.");
            if (room.Status != RoomStatus.Active) return ServiceResult.Failure("Room is not active.");

            var flashcardsInSet = await _unitOfWork.Flashcards.GetAllByFlashcardSetIdAsync(room.FlashcardSetId);
            if (flashcardsInSet == null || !flashcardsInSet.Any()) return ServiceResult.Failure("Flashcard set is empty.");

            int nextIndex = room.CurrentQuestionIndex + 1;
            if (nextIndex >= flashcardsInSet.Count())
            {
                _logger.LogInformation("AdvanceToNextQuestion: End of flashcards for Room {RoomId}.", roomId);
                return await FinishRoomAsync(roomId, teacherId, true); // Close the room
            }

            room.CurrentQuestionIndex = nextIndex;
            _unitOfWork.Rooms.Update(room);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Room {RoomId} advanced to question index {NextIndex}", roomId, nextIndex);

            var nextFlashcard = flashcardsInSet.OrderBy(f => f.FlashcardId).Skip(nextIndex).FirstOrDefault();
            if (nextFlashcard != null)
            {
                var questionDto = MapToFlashcardQuestionDto(nextFlashcard, room.Mode);
                await _roomHubContext.Clients.Group(roomId.ToString()).SendAsync("NextFlashcard", questionDto, room.CurrentQuestionIndex);
                return ServiceResult.Success("Advanced to next question.");
            }
            return ServiceResult.Failure("Could not retrieve next flashcard.");
        }

        public async Task<ServiceResult<RoomDto>> CreateRoomAsync(CreateRoomRequestDto request, Guid teacherId)
        {
            string teacherIdString = teacherId.ToString();
            var flashcardSet = await _unitOfWork.FlashcardSets.GetAsync(request.FlashcardSetId);

            if (flashcardSet == null)
            {
                _logger.LogWarning("CreateRoom: FlashcardSetId {FlashcardSetId} not found.", request.FlashcardSetId);
                return ServiceResult<RoomDto>.Failure("FlashcardSet not found.");
            }
            if (flashcardSet.UserId != teacherIdString)
            {
                _logger.LogWarning("CreateRoom: FlashcardSetId {FlashcardSetId} does not belong to teacher {TeacherId}.", request.FlashcardSetId, teacherId);
                return ServiceResult<RoomDto>.Failure("Unauthorized to use this FlashcardSet.");
            }
            if (!(await _unitOfWork.FlashcardSets.GetAsync(request.FlashcardSetId)).Flashcards.Any()) // Kiểm tra set có flashcard không
            {
                _logger.LogWarning("CreateRoom: FlashcardSetId {FlashcardSetId} has no flashcards.", request.FlashcardSetId);
                return ServiceResult<RoomDto>.Failure("FlashcardSet must contain at least one flashcard.");
            }

            string roomCode;
            do { roomCode = GenerateRandomRoomCode(6); }
            while (await _unitOfWork.Rooms.AnyAsync(r => r.RoomCode == roomCode));

            var room = _mapper.Map<Room>(request);
            room.RoomId = Guid.NewGuid();
            room.UserId = teacherIdString;
            room.RoomCode = roomCode;
            room.Status = RoomStatus.Pending;
            room.CurrentQuestionIndex = -1;
            room.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Room {RoomId} created with code {RoomCode} by teacher {TeacherId}", room.RoomId, room.RoomCode, teacherId);

            var roomDto = _mapper.Map<RoomDto>(room);

            roomDto.TeacherName = (await _unitOfWork.Auth.GetUserByIdAsync(teacherIdString))?.UserName;
            roomDto.FlashcardSetName = flashcardSet.Title;

            return ServiceResult<RoomDto>.Success(roomDto);
        }

        public async Task<ServiceResult> FinishRoomAsync(Guid roomId, Guid teacherId, bool triggeredByNoMoreQuestions = false)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            var teacherIdString = teacherId.ToString();

            if (room == null) return ServiceResult.Failure("Room not found.");
            if (!triggeredByNoMoreQuestions && room.UserId != teacherIdString) return ServiceResult.Failure("Unauthorized to finish this room.");
            if (room.Status == RoomStatus.Finished) return ServiceResult.Failure("Room already finished.");

            room.Status = RoomStatus.Finished;
            room.EndTime = DateTime.UtcNow;
            _unitOfWork.Rooms.Update(room);

            var participants = await _unitOfWork.RoomParticipants.GetParticipantsByRoomIdAsync(roomId);
            foreach (var p in participants)
            {
                p.Status = RoomParticipantStatus.Finished;
                _unitOfWork.RoomParticipants.Update(p);
            }
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Room {RoomId} finished.", roomId);

            var finalLeaderboard = await GetLeaderboarAsync(roomId);
            await _roomHubContext.Clients.Group(roomId.ToString()).SendAsync("RoomFinished", finalLeaderboard);
            return ServiceResult.Success("Room finished.");
        }

        public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboarAsync(Guid roomId)
        {
            var participants = await _unitOfWork.RoomParticipants.GetLeaderboardForRoomAsync(roomId);
            return _mapper.Map<IEnumerable<LeaderboardEntryDto>>(participants);
        }

        public async Task<RoomDetailsDto?> GetRoomDetailsAsync(Guid roomId)
        {
            var room = await _unitOfWork.Rooms.GetRoomWithDetailsAsync(roomId);

            if (room == null)
            {
                _logger.LogWarning("GetRoomDetails: Room {RoomId} not found.", roomId);
                return null;
            }

            var roomDetailsDto = _mapper.Map<RoomDetailsDto>(room);

            if (room.User != null) roomDetailsDto.TeacherName = room.User.UserName;
            if (room.FlashcardSet != null) roomDetailsDto.FlashcardSetName = room.FlashcardSet.Title;
            roomDetailsDto.CurrentParticipantCount = room.RoomParticipants.Count;

            roomDetailsDto.Participants = _mapper.Map<List<RoomParticipantDto>>(room.RoomParticipants);

            if (room.Status == RoomStatus.Active && room.CurrentQuestionIndex >= 0)
            {
                var flashcards = await _unitOfWork.Flashcards.GetAllByFlashcardSetIdAsync(room.FlashcardSetId);
                var currentFlashcard = flashcards?.OrderBy(f => f.FlashcardId).Skip(room.CurrentQuestionIndex).FirstOrDefault();
                if (currentFlashcard != null)
                {
                    roomDetailsDto.CurrentFlashcard = MapToFlashcardQuestionDto(currentFlashcard, room.Mode);
                }
            }

            return roomDetailsDto;

        }

        public async Task<ServiceResult<JoinRoomResponseDto>> JoinRoomAsync(string roomCode, Guid studentId)
        {
            var studentIdString = studentId.ToString();
            var room = await _unitOfWork.Rooms.GetByRoomCodeWithIncludesAsync(roomCode, r => r.RoomParticipants);

            if (room == null) return ServiceResult<JoinRoomResponseDto>.Failure("Room not found.");
            if (room.Status == RoomStatus.Finished) return ServiceResult<JoinRoomResponseDto>.Failure("This room has already finished.");

            if (room.MaxParticipants.HasValue && room.RoomParticipants.Count >= room.MaxParticipants.Value)
            {
                return ServiceResult<JoinRoomResponseDto>.Failure("Room is full.");
            }

            var existingParticipant = room.RoomParticipants.FirstOrDefault(p => p.UserId == studentIdString);


            if (room.RoomParticipants.Any(p => p.UserId == studentIdString))
            {
                _logger.LogInformation("Student {StudentId} is rejoining Room {RoomId}", studentId, room.RoomId);
                existingParticipant.Status = (room.Status == RoomStatus.Active) ? RoomParticipantStatus.Playing : RoomParticipantStatus.Joined;
                existingParticipant.LastActivityAt = DateTime.UtcNow;
                existingParticipant.CurrentQuestionIndex = (room.Status == RoomStatus.Active) ? existingParticipant.CurrentQuestionIndex : -1; 

                _unitOfWork.RoomParticipants.Update(existingParticipant);
                /*return ServiceResult<JoinRoomResponseDto>.Failure("You are already in this room.");*/
            }

            else
            {
                existingParticipant = new RoomParticipant
                {
                    RoomParticipantId = Guid.NewGuid(),
                    RoomId = room.RoomId,
                    UserId = studentIdString,
                    JoinedAt = DateTime.UtcNow,
                    Status = (room.Status == RoomStatus.Active) ? RoomParticipantStatus.Playing : RoomParticipantStatus.Joined,
                    LastActivityAt = DateTime.UtcNow,
                    CurrentQuestionIndex = (room.Status == RoomStatus.Active) ? 0 : -1, 
                };

                await _unitOfWork.RoomParticipants.AddAsync(existingParticipant);
            }

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Student {StudentId} joined Room {RoomId} (Code: {RoomCode})", studentId, room.RoomId, roomCode);

            var studentUser = await _unitOfWork.Auth.GetUserByIdAsync(studentIdString);
            var participantDtoForSignalR = _mapper.Map<RoomParticipantDto>(existingParticipant);
            participantDtoForSignalR.Username = studentUser?.UserName;

            await _roomHubContext.Clients.Group(room.RoomId.ToString()).SendAsync("UserJoined", participantDtoForSignalR);

            var roomDto = _mapper.Map<RoomDto>(room);
            roomDto.TeacherName = (await _unitOfWork.Auth.GetUserByIdAsync(room.UserId))?.UserName;
            roomDto.FlashcardSetName = (await _unitOfWork.FlashcardSets.GetAsync(room.FlashcardSetId))?.Title;
            /*roomDto.CurrentParticipantCount = room.RoomParticipants.Count + 1;*/


            // LẤY DANH SÁCH TẤT CẢ PARTICIPANTS HIỆN TẠI TRONG PHÒNG
            var allParticipants = await _unitOfWork.RoomParticipants.GetParticipantsByRoomIdAsync(room.RoomId);
            var currentParticipantsInRoomDto = new List<RoomParticipantDto>();
            foreach (var p in allParticipants.Where(pDb => pDb.Status != RoomParticipantStatus.Left))
            {
                var pUser = await _unitOfWork.Auth.GetUserByIdAsync(p.UserId);
                var pDto = _mapper.Map<RoomParticipantDto>(p);
                pDto.Username = pUser?.UserName;
                currentParticipantsInRoomDto.Add(pDto);
            }
            roomDto.CurrentParticipantCount = currentParticipantsInRoomDto.Count;




            /*var participantDto = _mapper.Map<RoomParticipantDto>(existingParticipant);
            participantDto.Username = studentUser?.UserName;*/

            FlashcardQuestionDto? currentQuestionDto = null;

            if (room.Status == RoomStatus.Active && room.CurrentQuestionIndex >= 0)
            {
                var flashcards = await _unitOfWork.Flashcards.GetAllByFlashcardSetIdAsync(room.FlashcardSetId);
                var currentFlashcard = flashcards?.OrderBy(f => f.FlashcardId).Skip(existingParticipant.CurrentQuestionIndex).FirstOrDefault();
                if (currentFlashcard != null)
                {
                    currentQuestionDto = MapToFlashcardQuestionDto(currentFlashcard, room.Mode);
                }
            }

            var response = new JoinRoomResponseDto { 
                RoomInfo = roomDto, 
                ParticipantInfo = participantDtoForSignalR, 
                CurrentQuestion = currentQuestionDto,
                CurrentParticipantsInRoom = currentParticipantsInRoomDto,
            };
            return ServiceResult<JoinRoomResponseDto>.Success(response);

        }

        public async Task<ServiceResult<AnswerProcessingResultDto>> ProcessAnswerAsync(Guid roomId, Guid studentId, int flashCardIdFromClient, string answer)
        {
            var studentIdString = studentId.ToString();
            var participant = await _unitOfWork.RoomParticipants.GetParticipantInRoomAsync(roomId, studentId);

            if (participant == null) return ServiceResult<AnswerProcessingResultDto>.Failure("Participant not found.");
            if (participant.Status != RoomParticipantStatus.Playing) return ServiceResult<AnswerProcessingResultDto>.Failure("You are not actively playing or have finished.");

            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null || room.Status != RoomStatus.Active) return ServiceResult<AnswerProcessingResultDto>.Failure("Room is not active.");

            var flashcardsInSet = await _unitOfWork.Flashcards.GetAllByFlashcardSetIdAsync(room.FlashcardSetId);
            if (flashcardsInSet == null || !flashcardsInSet.Any()) return ServiceResult<AnswerProcessingResultDto>.Failure("Flashcard set not found or empty.");

            var currentFlashcard = flashcardsInSet.OrderBy(f => f.FlashcardId).Skip(participant.CurrentQuestionIndex).FirstOrDefault();

            if (currentFlashcard == null) 
            {
                _logger.LogWarning("Student {StudentId} in Room {RoomId} seems to be beyond last question. Index: {Index}", studentId, roomId, participant.CurrentQuestionIndex);
                participant.Status = RoomParticipantStatus.Finished; 
                _unitOfWork.RoomParticipants.Update(participant);
                await _unitOfWork.CompleteAsync();
                return ServiceResult<AnswerProcessingResultDto>.Failure("No more questions available or error retrieving current question.");
            } 


            if (currentFlashcard.FlashcardId != flashCardIdFromClient) return ServiceResult<AnswerProcessingResultDto>.Failure("Answer submitted for an incorrect or outdated question. Please refresh if you see this often.");

            bool isCorrect = CheckAnswer(currentFlashcard, answer, room.Mode);

            var attempt = new StudentFlashcardAttempt
            {
                AttemptId = Guid.NewGuid(),
                RoomParticipantId = participant.RoomParticipantId,
                FlashcardId = currentFlashcard.FlashcardId,
                RoomId = roomId,
                AnswerText = answer,
                IsCorrect = isCorrect,
                SubmittedAt = DateTime.UtcNow,
            };
            await _unitOfWork.StudentFlashcardAttempts.AddAsync(attempt);

            // Add the score to the participant
            if (isCorrect)
            {
                double secondsAns = (DateTime.UtcNow - participant.LastActivityAt.Value).TotalSeconds;
                participant.Score += CalculateScore(isCorrect, secondsAns);

            }
            participant.LastActivityAt = DateTime.UtcNow;


            FlashcardQuestionDto? nextQuestionForThisStudentDto = null;
            int nextPersonalIndex = participant.CurrentQuestionIndex + 1;
            bool hasFinished = false;

            if (nextPersonalIndex < flashcardsInSet.Count())
            {
                participant.CurrentQuestionIndex = nextPersonalIndex; // Cập nhật index mới cho participant
                var nextFlashcardEntity = flashcardsInSet.OrderBy(f => f.FlashcardId).Skip(nextPersonalIndex).FirstOrDefault();
                if (nextFlashcardEntity != null)
                {
                    nextQuestionForThisStudentDto = MapToFlashcardQuestionDto(nextFlashcardEntity, room.Mode);
                }
            }
            else
            {
                // Student has completed all flashcards
                _logger.LogInformation("Student {StudentId} in Room {RoomId} has completed all flashcards.", studentId, roomId);
                participant.Status = RoomParticipantStatus.Finished; 
                participant.LastAnswerSubmittedAt = DateTime.UtcNow;
                hasFinished = true; 
            }

            _unitOfWork.RoomParticipants.Update(participant); 
            await _unitOfWork.CompleteAsync();



            _logger.LogInformation("Student {StudentId} in Room {RoomId} answered. Correct: {IsCorrect}. Score: {Score}. New Personal Index: {NewIndex}",
                studentId, roomId, isCorrect, participant.Score, participant.CurrentQuestionIndex);

            var resultDto = new AnswerProcessingResultDto
            {
                IsCorrect = isCorrect,
                CorrectAnswer = (room.Mode == RoomMode.TermToDefinition) ? currentFlashcard.Definition : currentFlashcard.Term,
                NewScore = participant.Score,
                NextQuestion = nextQuestionForThisStudentDto, 
                HasFinished = hasFinished
            };


            var leaderboard = await GetLeaderboarAsync(roomId);
            await _roomHubContext.Clients.Group(roomId.ToString()).SendAsync("LeaderboardUpdate", leaderboard);

            return ServiceResult<AnswerProcessingResultDto>.Success(resultDto);
        }

        public async Task<ServiceResult> StartRoomAsync(Guid roomId, Guid teacherId)
        {
            var room = await _unitOfWork.Rooms.GetRoomWithDetailsAsync(roomId);
            var teacherIdString = teacherId.ToString();

            if (room == null) return ServiceResult.Failure("Room not found.");
            if (room.UserId != teacherIdString) return ServiceResult.Failure("Unauthorized to start this room.");
            if (room.Status != RoomStatus.Pending) return ServiceResult.Failure("Room can only be started if pending.");
            if (!room.RoomParticipants.Any()) return ServiceResult.Failure("Cannot start an empty room. At least one participant is required.");

            var flashcards = await _unitOfWork.Flashcards.GetAllByFlashcardSetIdAsync(room.FlashcardSetId);
            if (flashcards == null || !flashcards.Any())
            {
                _logger.LogError("StartRoom: FlashcardSet {FlashcardSetId} for Room {RoomId} is empty.", room.FlashcardSetId, roomId);
                return ServiceResult.Failure("Flashcard set is empty.");
            }

            room.Status = RoomStatus.Active;
            room.StartTime = DateTime.UtcNow;
            room.CurrentQuestionIndex = 0;

            foreach (var participant in room.RoomParticipants)
            {
                participant.Status = RoomParticipantStatus.Playing;
                participant.CurrentQuestionIndex = 0;
                participant.LastActivityAt = DateTime.UtcNow;
               /* _unitOfWork.RoomParticipants.Update(participant);*/
            }

            /*_unitOfWork.Rooms.Update(room);*/
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Room {RoomId} started. Index: {Index}", roomId, room.CurrentQuestionIndex);

            var firstFlashcard = flashcards.OrderBy(f => f.FlashcardId).FirstOrDefault(); 
            if (firstFlashcard != null)
            {
                var questionDto = MapToFlashcardQuestionDto(firstFlashcard, room.Mode);
                await _roomHubContext.Clients.Group(roomId.ToString()).SendAsync("NextFlashcard", questionDto, room.CurrentQuestionIndex);
            }
            return ServiceResult.Success("Room started.");
        }

        private string GenerateRandomRoomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private FlashcardQuestionDto MapToFlashcardQuestionDto(Flashcard flashcard, RoomMode mode)
        {
            return new FlashcardQuestionDto
            {
                FlashcardId = flashcard.FlashcardId,
                QuestionText = (mode == RoomMode.TermToDefinition) ? flashcard.Term : flashcard.Definition,
                ExampleSentence = flashcard.Example
                /*ImageUrl = flashcard.ImageUrl*/
            };
        }

        private bool CheckAnswer(Flashcard currentFlashcard, string answerText, RoomMode mode)
        {
            string correctAnswer = (mode == RoomMode.TermToDefinition) ? currentFlashcard.Definition : currentFlashcard.Term;
            return string.Equals(correctAnswer.Trim(), answerText.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private int CalculateScore(bool isCorrect, double secondsAns)
        {
            const double thresholdTime = 15.0; 
            const double maxPenaltyTime = 30.0; 
            const int minScore = 4;  
            const int maxScore = 10;

            if (!isCorrect) return 0;

            if (secondsAns <= thresholdTime)
            {
                return maxScore;
            }
            else if (secondsAns <= maxPenaltyTime)
            {
                double overtime = secondsAns - thresholdTime;
                double penaltyRatio = overtime / (maxPenaltyTime - thresholdTime);
                int penalty = (int)Math.Floor((maxScore - minScore) * penaltyRatio);
                return Math.Max(minScore, maxScore - penalty); 
            }
            else
            {
                return minScore;
            }
        }

        public async Task<bool> IsUserParticipantInRoomAsync(Guid roomId, Guid userId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room != null && room.UserId == userId.ToString())
            {
                _logger.LogInformation("User {UserId} is the teacher of Room {RoomId}. Authorized for SignalR group.", userId, roomId);
                return true; 
            }
            return await _unitOfWork.RoomParticipants.AnyAsync(rp => rp.RoomId == roomId && rp.UserId == userId.ToString() && rp.Status != RoomParticipantStatus.Left);
        }

        public async Task<UserLeftRoomDetailDto?> HandleUserDisconnectAsync(Guid userId, string connectionId)
        {
            string userIdString = userId.ToString();
            _logger.LogInformation("Handling disconnect for User {UserIdString}, Connection {ConnectionId}", userIdString, connectionId);

            var participantEntity = await _dbContext.RoomParticipants
                .Include(p => p.Room)
                .Include(p => p.Room.User)
                .Where(rp => rp.UserId == userIdString && rp.Status != RoomParticipantStatus.Finished && 
                    (rp.Status == RoomParticipantStatus.Playing || rp.Status == RoomParticipantStatus.Joined))
                .OrderByDescending(rp => rp.LastActivityAt)
                .ThenByDescending(rp => rp.JoinedAt)       
                .FirstOrDefaultAsync();

            if (participantEntity == null)
            {
                _logger.LogInformation("No active (non-finished room, playing/joined status) room participation found for disconnected User {UserIdString}", userIdString);
                return null;
            }

            _logger.LogInformation("User {UserIdString} (ParticipantId: {ParticipantId}) was active in Room {RoomId}. Current DB status: {Status}. Room Status: {RoomStatus}",
            userIdString, participantEntity.RoomParticipantId, participantEntity.RoomId, participantEntity.Status, participantEntity.Room.Status);

            var leftParticipantDto = _mapper.Map<RoomParticipantDto>(participantEntity);

            return new UserLeftRoomDetailDto
            {
                RoomId = participantEntity.RoomId,
                LeftParticipant = leftParticipantDto
            };
        }
    }
}
