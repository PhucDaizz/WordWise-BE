namespace WordWise.Api.Services.Interface
{
    public interface IRoomDataCleaner
    {
        Task CleanUpRoomDataAsync(Guid roomId);
    }
}
