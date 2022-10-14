namespace GusMelford.TelegramBot.Domain.Telegram;

public static class Convertor
{
    public static UpdateDomain ToDomain(this TBot.Telegram.Dto.Update update)
    {
        return new UpdateDomain(update.UpdateId, update.Message?.ToDomain());
    }
    
    private static MessageDomain ToDomain(this TBot.Telegram.Dto.UpdateModule.Message message)
    {
        return new MessageDomain(
            message.MessageId,
            message.From?.ToDomain(),
            message.Date,
            message.Chat.ToDomain(),
            message.ForwardFromUser?.ToDomain(),
            message.ForwardDate,
            message.MigrateToChatId,
            message.MigrateFromChatId,
            message.Text,
            message.Caption,
            message.NewChatTitle,
            message.DeleteChatPhoto,
            message.GroupChatCreated,
            message.SupergroupChatCreated,
            message.ChannelChatCreated,
            message.NewChatMember?.ToDomain(),
            message.LeftChatMember?.ToDomain(),
            message.PinnedMessage?.ToDomain(),
            message.ReplyToMessage?.ToDomain());
    }

    private static UserDomain ToDomain(this TBot.Telegram.Dto.User user)
    {
        return new UserDomain(user.Id, user.FirstName, user.LastName, user.Username);
    }

    private static ChatDomain ToDomain(this TBot.Telegram.Dto.Chat chat)
    {
        return new ChatDomain(
            chat.Id, 
            chat.Type, 
            chat.Title, 
            chat.Username, 
            chat.FirstName, 
            chat.LastName,
            chat.AllMembersAreAdministrators);
    }
}