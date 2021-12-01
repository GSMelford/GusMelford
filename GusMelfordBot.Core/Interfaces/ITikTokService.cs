namespace GusMelfordBot.Core.Interfaces
{
    using Telegram.Dto.UpdateModule;
    using System.Threading.Tasks;
    
    public interface ITikTokService
    {
        Task SendVideoInfo();
        Task DeleteVideoInfo();
        void SaveVideoAsync(Message message);
    }
}