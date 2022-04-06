using System.Linq;
using System.Threading.Tasks;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Core.Applications.MemesChatApp;

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
            await SetAccompanyingCommentary(message);
        }
    }

    public async Task ProcessCallbackQuery(CallbackQuery updateCallbackQuery)
    {
        await _playerService.ProcessCallbackQuery(updateCallbackQuery);
    }

    private ContentProvider SelectProvider(string messageText)
    {
        return messageText.Contains(Constants.TikTok) ? ContentProvider.TikTok : ContentProvider.Other;
    }

    private async Task SetAccompanyingCommentary(Message message)
    {
        TikTokVideoContent tikTokVideoContent = await _databaseManager.Context.Set<TikTokVideoContent>()
            .FirstOrDefaultAsync(x => x.MessageId == message.ReplyToMessage.MessageId);

        if (tikTokVideoContent is not null)
        {
            tikTokVideoContent.AccompanyingCommentary = message.Text;
        }

        await _databaseManager.Context.SaveChangesAsync();
    }
}

public enum ContentProvider
{
    TikTok,
    YouTube,
    Other
}