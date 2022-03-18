using System.Linq;
using System.Threading.Tasks;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
using GusMelfordBot.Database.Interfaces;

namespace GusMelfordBot.Core.Applications.MemesChatApp
{
    using ContentProviders.TikTok;
    using Interfaces;
    using Telegram.Dto.UpdateModule;
    
    public class MemeChatService : IMemeChatService
    {
        private readonly ITikTokService _tikTokService;
        private readonly IPlayerService _playerService;
        private readonly IDatabaseManager _databaseManager;
        
        public MemeChatService(
            IDatabaseManager databaseManager,
            ITikTokService tikTokService, 
            IPlayerService playerService)
        {
            _tikTokService = tikTokService;
            _playerService = playerService;
            _databaseManager = databaseManager;
        }
        
        public async Task ProcessMessage(Message message)
        {
            string messageText = message.Text;
            switch (SelectProvider(messageText))
            {
                case ContentProvider.TikTok:
                    await _tikTokService.ProcessMessage(message);
                    break;
            }

            if (message.ReplyToMessage is not null)
            {
                SetAccompanyingCommentary(message);
            }
        }

        public void ProcessCallbackQuery(CallbackQuery updateCallbackQuery)
        {
            _playerService.ProcessCallbackQuery(updateCallbackQuery);
        }

        private ContentProvider SelectProvider(string messageText)
        {
            return messageText.Contains(Constants.TikTok) ? ContentProvider.TikTok : ContentProvider.Other;
        }

        private void SetAccompanyingCommentary(Message message)
        {
            TikTokVideoContent tikTokVideoContent = _databaseManager.Context.Set<TikTokVideoContent>()
                .FirstOrDefault(x => x.MessageId == message.ReplyToMessage.MessageId);

            if (tikTokVideoContent is not null)
            {
                tikTokVideoContent.AccompanyingCommentary = message.Text;
            }

            _databaseManager.Context.SaveChanges();
        }
    }

    public enum ContentProvider
    {
        TikTok,
        YouTube,
        Other
    }
}