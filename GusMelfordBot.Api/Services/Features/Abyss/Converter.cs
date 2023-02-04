using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;

namespace GusMelfordBot.Api.Services.Features.Abyss;

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
            contentProcessedEvent.GroupId, 
            new List<Guid> { contentProcessedEvent.UserId },
            contentProcessedEvent.Provider ?? throw new Exception(),
            contentProcessedEvent.OriginalLink ?? throw new Exception(),
            contentProcessedEvent.ToMetaContentDomain(),
            !string.IsNullOrEmpty(contentProcessedEvent.UserComment)
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
        return new UserComment(contentProcessedEvent.UserId, contentProcessedEvent.UserComment!);
    }

    public static AttemptContent ToDomain(this AttemptContentEvent attemptContentEvent)
    {
        return new AttemptContent(
            attemptContentEvent.SessionId,
            attemptContentEvent.GroupId,
            attemptContentEvent.UserId, 
            attemptContentEvent.Message,
            attemptContentEvent.Attempt);
    }
}