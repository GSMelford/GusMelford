using System.Threading.Tasks;
using GusMelfordBot.Core.Applications.Commander.Commands;
using GusMelfordBot.Core.Interfaces;
using GusMelfordBot.Database.Interfaces;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Applications.Commander;

public class CommanderService : ICommanderService
{
    private readonly IDatabaseManager _databaseManager;
    private readonly IGusMelfordBotService _gusMelfordBotService;
        
    public CommanderService(
        IDatabaseManager databaseManager, 
        IGusMelfordBotService gusMelfordBotService)
    {
        _databaseManager = databaseManager;
        _gusMelfordBotService = gusMelfordBotService;
    }
        
    public async Task ProcessMessage(Message message)
    {
        string command = message.Text.Replace(CommandConst.Determinant, "").Trim();
        switch (command)
        {
            case CommandConst.RegisterMemesChat:
                await RegisterMemesChat.Register(_databaseManager, _gusMelfordBotService, message.Chat);
                break;
        }
    }
}