
using WordWise.Api.Repositories.Interface;
using WordWise.Api.Services.Interface;

namespace WordWise.Api.BackgroundServices
{
    public class OldRoomDataCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OldRoomDataCleanupService> _logger;
        private readonly TimeSpan _retentionPeriod = TimeSpan.FromMinutes(2);
        private readonly TimeSpan _checkTimePending = TimeSpan.FromMinutes(40);
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

        public OldRoomDataCleanupService(IServiceProvider serviceProvider, ILogger<OldRoomDataCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OldRoomDataCleanupService is starting.");
            stoppingToken.Register(() => _logger.LogInformation("OldRoomDataCleanupService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("OldRoomDataCleanupService cycle starting at: {time}", DateTimeOffset.Now);

                try
                {
                    // Tạo một scope mới cho mỗi lần chạy để resolve Scoped services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
                        var roomDataCleaner = scope.ServiceProvider.GetRequiredService<IRoomDataCleaner>();

                        // Lấy danh sách các phòng đã hoàn thành và quá thời gian lưu trữ
                        var roomsToClean = await roomRepository.GetFinishedRoomsOlderThanAsync(_retentionPeriod);

                        // Lấy danh sách các phòng đang trong tình trạng chờ quá lâu
                        var waitingRooms = await roomRepository.GetWaitingRoomsOlderThanAsync(_checkTimePending);

                        if (roomsToClean.Any() || waitingRooms.Any())
                        {
                            _logger.LogInformation("Found {Count} old rooms to clean up.", roomsToClean.Count());
                            _logger.LogInformation("Found {Count} rooms to clean up in status pending too long.", roomsToClean.Count());
                            foreach (var room in roomsToClean)
                            {
                                if (stoppingToken.IsCancellationRequested) break; // Kiểm tra trước mỗi lần xóa

                                _logger.LogInformation("Cleaning up Room ID: {RoomId}, Ended At: {EndTime}", room.RoomId, room.EndTime);
                                await roomDataCleaner.CleanUpRoomDataAsync(room.RoomId);
                            }

                            foreach (var room in waitingRooms)
                            {
                                if (stoppingToken.IsCancellationRequested) break;

                                _logger.LogInformation("Cleaning up waiting Room ID: {RoomId}, Created At: {CreatedTime}", room.RoomId, room.CreatedAt);
                                await roomDataCleaner.CleanUpRoomDataAsync(room.RoomId);
                            }

                            _logger.LogInformation("Finished cleaning up old rooms for this cycle.");
                        }
                        else
                        {
                            _logger.LogInformation("No old rooms found to clean up in this cycle.");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("OldRoomDataCleanupService cleanup operation was canceled during processing.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the OldRoomDataCleanupService cycle.");
                }

                try
                {
                    _logger.LogInformation("OldRoomDataCleanupService waiting for {CheckInterval} before next cycle.", _checkInterval);
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("OldRoomDataCleanupService delay was canceled, service is stopping.");
                    break;
                }
            }
            _logger.LogInformation("OldRoomDataCleanupService has stopped.");
        }
    }
}

