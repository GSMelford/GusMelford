namespace GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok
{
    using System.Threading.Tasks;
    using Telegram.Dto.UpdateModule;
    
    public interface ITikTokService
    {
        Task ProcessMessage(Message message);
    }
}