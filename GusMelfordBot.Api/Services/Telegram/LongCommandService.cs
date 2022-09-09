using GusMelfordBot.Domain.Telegram;

namespace GusMelfordBot.Api.Services.Telegram;

public class LongCommandService : ILongCommandService
{
    private readonly List<LongCommand> _processedCommands = new ();

    public LongCommand? GetLongCommand(long telegramId)
    {
        return _processedCommands.FirstOrDefault(x => x.TelegramId == telegramId);
    }
    
    public bool TryAdd(long telegramId, string commandName)
    {
        LongCommand? longCommand = _processedCommands.FirstOrDefault(x => x.TelegramId == telegramId);
        if (longCommand is null)
        {
            _processedCommands.Add(new LongCommand
            {
                Name = commandName,
                TelegramId = telegramId
            });
            
            return true;
        }
        
        _processedCommands.Remove(longCommand);
        return false;
    }

    public void Remove(long telegramId)
    {
        _processedCommands.Remove(_processedCommands.FirstOrDefault(x => x.TelegramId == telegramId)!);
    }
}