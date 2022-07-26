using GusMelfordBot.Domain.Telegram.Models;
using TBot.Telegram.Dto;
using TBot.Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Api.Dto.Telegram;

public static class Convertor
{
    public static UpdateDomain ToDomain(this Update update)
    {
        return new UpdateDomain(update.UpdateId, update.Message?.ToDomain());
    }

    private static MessageDomain ToDomain(this Message message)
    {
        return new MessageDomain(
            message.MessageId,
            message.Text, 
            message.Caption,
            message.From?.ToDomain(),
            message.Chat.ToDomain());
    }

    private static UserDomain ToDomain(this User user)
    {
        return new UserDomain(user.Id, user.FirstName, user.LastName, user.Username);
    }

    private static ChatDomain ToDomain(this Chat chat)
    {
        return new ChatDomain(chat.Id);
    }
}