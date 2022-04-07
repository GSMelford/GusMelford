namespace GusMelfordBot.Core.Applications;

using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
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

    public async Task DefineApplicationFromMessage(Message message)
    {
        await AddUserIfNotExist(message);
        if (IsCommand(message))
        {
            await _commanderService.ProcessMessage(message);
        }

        if (IsMemesChatService(message))
        {
            await _memeChatService.ProcessMessage(message);
        }
    }

    public async Task DefineApplicationFromCallbackQuery(CallbackQuery updateCallbackQuery)
    {
        if (IsMemesChatService(updateCallbackQuery.Message))
        {
            await _memeChatService.ProcessCallbackQuery(updateCallbackQuery);
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

    private async Task AddUserIfNotExist(Message message)
    {
        User user = await _databaseManager.Context.Set<User>()
            .FirstOrDefaultAsync(x => x.UserName == message.From.Username
                                      || x.TelegramUserId == message.From.Id);

        if (user is null)
        {
            await _databaseManager.Context.AddAsync(
                new User
                {
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    UserName = message.From.Username,
                    TelegramUserId = message.From.Id
                });

            await _databaseManager.Context.SaveChangesAsync();
        }
    }
}