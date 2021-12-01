namespace GusMelfordBot.Core.Interfaces
{
    using Services.PlayerServices;
    using DAL.TikTok;
    using System.Threading.Tasks;
    
    public interface IPlayerService
    {
        Task Start();
        Task<VideoInfo> SetNextVideo();
        Task<VideoInfo> SetPreviousVideo();
        Video CurrentVideo { get; }
        VideoFile CurrentVideoFile { get; }
    }
}