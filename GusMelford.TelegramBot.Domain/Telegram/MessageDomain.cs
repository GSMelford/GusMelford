namespace GusMelford.TelegramBot.Domain.Telegram;

public class MessageDomain
{
    public int MessageId { get; }
    public UserDomain? From { get; }
    public int Date { get; }
    public ChatDomain Chat { get; }
    public UserDomain? ForwardFromUser { get; }
    public int? ForwardDate { get; }
    public int? MigrateToChatId { get; }
    public int? MigrateFromChatId { get; }
    public string? Text { get; }
    public string? Caption { get; }
    public string? NewChatTitle { get; }
    public bool? DeleteChatPhoto { get; }
    public bool? GroupChatCreated { get; }
    public bool? SupergroupChatCreated { get; }
    public bool? ChannelChatCreated { get; }
    public UserDomain? NewChatMember { get; }
    public UserDomain? LeftChatMember { get; }
    public MessageDomain? PinnedMessage { get; }
    public MessageDomain? ReplyToMessage { get; }

    public MessageDomain(
        int messageId,
        UserDomain? from, 
        int date, 
        ChatDomain chat,
        UserDomain? forwardFromUser, 
        int? forwardDate, 
        int? migrateToChatId,
        int? migrateFromChatId, 
        string? text, 
        string? caption, 
        string? newChatTitle, 
        bool? deleteChatPhoto,
        bool? groupChatCreated, 
        bool? supergroupChatCreated, 
        bool? channelChatCreated, 
        UserDomain? newChatMember, 
        UserDomain? leftChatMember, 
        MessageDomain? pinnedMessage, 
        MessageDomain? replyToMessage)
    {
        MessageId = messageId;
        From = from;
        Date = date;
        Chat = chat;
        ForwardFromUser = forwardFromUser;
        ForwardDate = forwardDate;
        MigrateToChatId = migrateToChatId;
        MigrateFromChatId = migrateFromChatId;
        Text = text;
        Caption = caption;
        NewChatTitle = newChatTitle;
        DeleteChatPhoto = deleteChatPhoto;
        GroupChatCreated = groupChatCreated;
        SupergroupChatCreated = supergroupChatCreated;
        ChannelChatCreated = channelChatCreated;
        NewChatMember = newChatMember;
        LeftChatMember = leftChatMember;
        PinnedMessage = pinnedMessage;
        ReplyToMessage = replyToMessage;
    }
}