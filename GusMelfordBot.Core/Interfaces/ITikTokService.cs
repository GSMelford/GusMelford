namespace GusMelfordBot.Core.Interfaces
{
    using Telegram.Dto.UpdateModule;
    using System.Threading.Tasks;
    
    public interface ITikTokService
    {
        Task SendVideoInfo();
        Task DeleteVideoInfo();
        void ProcessMessage(Message message);
        public DAL.TikTok.Video CreateVideo(string link, long userId = 443763853);
    }
}