using System.Threading.Tasks;
using GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok;

namespace GusMelfordBot.Core.Applications.MemesChatApp
{
    using Interfaces;
    using Telegram.Dto.UpdateModule;
    
    public class MemeChatService : IMemeChatService
    {
        private const string TIK_TOK_VM_DOMAIN = "https://vm.tiktok.com/";
        private const string TIK_TOK_M_DOMAIN = "https://m.tiktok.com/";
        private const string TIK_TOK_WWW_DOMAIN = "https://www.tiktok.com/";

        private readonly ITikTokService _tikTokService;
        
        public MemeChatService(ITikTokService tikTokService)
        {
            _tikTokService = tikTokService;
        }
        
        public void ProcessMessage(Message message)
        {
            string messageText = message.Text;
            switch (SelectProvider(messageText))
            {
                case ContentProvider.TikTok:
                    _tikTokService.ProcessMessage(message);
                    break;
            }
        }

        private ContentProvider SelectProvider(string messageText)
        {
            if (messageText.Contains(TIK_TOK_VM_DOMAIN) 
                || messageText.Contains(TIK_TOK_M_DOMAIN) 
                || messageText.Contains(TIK_TOK_WWW_DOMAIN))
            {
                return ContentProvider.TikTok;
            }

            return ContentProvider.Other;
        }
    }

    public enum ContentProvider
    {
        TikTok,
        YouTube,
        Other
    }
}