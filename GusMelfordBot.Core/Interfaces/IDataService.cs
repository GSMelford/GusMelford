namespace GusMelfordBot.Core.Interfaces
{
    using Services.Data;
    using System.Threading.Tasks;
    
    public interface IDataService
    {
        Task<VideoData> GetUnwatchTikTokVideo();
        Task<VideoData> GetTikTokVideo(string takeDateSince, string takeDateUntil);
    }
}