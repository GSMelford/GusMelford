using GusMelford.TelegramBot.Domain.Telegram;
using TBot.Client;
using TBot.Client.Api.Telegram.SendMessage;

namespace GusMelford.TelegramBot.Services.Update.MessageHandlers.Abstractions.Command;

public class CommandHandler : AbstractMessageHandler
{
    private const char COMMAND_FORWARD_SLASH = '/';
    private const string BOT_NAME = "@GusMelfordBot";

    private readonly ITBot _bot;
    
    public CommandHandler(ITBot bot)
    {
        _bot = bot;
    }
    
    public override async Task<MessageDomain?> Handle(MessageDomain messageDomain)
    {
        string? messageText = messageDomain.Text;
        if (messageText?.FirstOrDefault() != COMMAND_FORWARD_SLASH)
        {
            return await base.Handle(messageDomain);
        }

        switch (GetClearCommand(messageText))
        {
            case Commands.RegisterContentChat:
                await RegisterChatAsContent(messageDomain.Chat);
                break;
        }

        return messageDomain;
    }

    private string GetClearCommand(string messageText)
    {
        return messageText.Replace(BOT_NAME, string.Empty);
    }

    private async Task RegisterChatAsContent(ChatDomain chatDomain)
    {
        //TODO Request to Core
        string result = string.Empty;
        string reason = string.Empty;
        
        switch (result)
        {
            case "NewChat":
                await _bot.SendMessageAsync(new SendMessageParameters
                {
                    Text =  "🥳 Congratulations!\n" +
                            $"Your {chatDomain.Title} is registered as a chat for collecting various content!\n" +
                            "🫵 Share content with friends and watch it together at app.gusmelford.com",
                    ChatId = chatDomain.Id
                });
                break;
            case "ExistChat":
                await _bot.SendMessageAsync(new SendMessageParameters
                {
                    Text =  "🤓 The chat is already registered as a content collector)!\n" +
                            "🫵 Share content with friends and watch it together at app.gusmelford.com",
                    ChatId = chatDomain.Id
                });
                break;
            case "CannotCreateChat":
                await _bot.SendMessageAsync(new SendMessageParameters
                {
                    Text =  "🫣 This chat cannot be registered as a content collector\n" +
                            $"👉 Reason: {reason}",
                    ChatId = chatDomain.Id
                });
                break;
        }
    }
}