using System.Linq;

namespace GusMelfordBot.Core.Applications
{
    using Commander;
    using GusMelfordBot.Core.Applications.MemesChatApp.Interfaces;
    using GusMelfordBot.DAL.Applications.MemesChat;
    using GusMelfordBot.Database.Interfaces;
    using Telegram.Dto.UpdateModule;
    
    public class ApplicationService : IApplicationService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly ICommanderService _commanderService;
        private readonly IMemeChatService _memeChatService;
        
        public ApplicationService(
            IDatabaseManager databaseManager,
            ICommanderService commanderService,
            IMemeChatService memeChatService)
        {
            _databaseManager = databaseManager;
            _memeChatService = memeChatService;
            _commanderService = commanderService;
        }
        
        public void DefineApplicationFromMessage(Message message)
        {
            if (IsCommand(message))
            {
                _commanderService.ProcessMessage(message);
            }
            if (IsMemesChatService(message))
            {
                _memeChatService.ProcessMessage(message);
            }
        }

        private bool IsCommand(Message message)
        {
            return message.Text.Contains(CommandConst.Determinant);
        }
        
        private bool IsMemesChatService(Message message)
        {
            MemesChat memesChat = _databaseManager.Context.Set<MemesChat>()
                .FirstOrDefault(x => x.ChatId == message.Chat.Id);
            return memesChat is not null;
        }
    }
}