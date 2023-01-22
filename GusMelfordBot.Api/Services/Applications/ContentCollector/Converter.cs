using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public static class Converter
{
    public static AbyssContext ToDomain(this TelegramMessageReceivedEvent telegramMessageReceivedEvent)
    {
        return new AbyssContext(
            telegramMessageReceivedEvent.SessionId,
            telegramMessageReceivedEvent.Message!.Text!,
            telegramMessageReceivedEvent.Message!.From!.Id!.Value,
            telegramMessageReceivedEvent.Message.Chat!.Id!.Value,
            telegramMessageReceivedEvent.Message.MessageId!.Value);
    }

    public static Content ToDomain(this ContentProcessedEvent contentProcessedEvent)
    {
        return new Content(
            contentProcessedEvent.SessionId,
            Guid.Parse(contentProcessedEvent.GroupId), 
            new List<Guid> { Guid.Parse(contentProcessedEvent.UserId) },
            contentProcessedEvent.Provider ?? throw new Exception(),
            contentProcessedEvent.OriginalLink ?? throw new Exception(),
            contentProcessedEvent.ToMetaContentDomain(),
            contentProcessedEvent.UserComment is not null
                ? new List<UserComment> { contentProcessedEvent.ToUserCommentDomain() }
                : new List<UserComment>());
    }

    public static MetaContent ToMetaContentDomain(this ContentProcessedEvent contentProcessedEvent)
    {
        return new MetaContent(
            contentProcessedEvent.IsSaved, 
            contentProcessedEvent.Height, 
            contentProcessedEvent.Width,
            contentProcessedEvent.Duration);
    }

    public static UserComment ToUserCommentDomain(this ContentProcessedEvent contentProcessedEvent)
    {
        return new UserComment(Guid.Parse(contentProcessedEvent.UserId), contentProcessedEvent.UserComment!);
    }
}