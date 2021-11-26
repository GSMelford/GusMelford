namespace GusMelfordBot.Core.Interfaces
{
    using Services.PlayerServices;
    using DAL.TikTok;
    using System.IO;
    using System.Threading.Tasks;
    
    public interface IPlayerService
    {
        Task Start();
        Task<VideoInfo> SetNextVideo();
        Task<VideoInfo> SetPreviousVideo();
        Task<Stream> GetCurrentVideoFileStream();
        Video GetCurrentVideo();
        VideoInfo GetVideoInfo();
        Stream GetCurrentVideoStream();
    }
}