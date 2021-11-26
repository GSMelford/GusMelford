namespace GusMelfordBot.Core.Interfaces
{
    using System.Threading.Tasks;
    using Services.PlayerServices;
    using DAL.TikTok;
    
    public interface IVideoDownloadService
    {
        Task<VideoFile> DownloadVideo(Video video);
    }
}