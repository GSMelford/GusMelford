using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.KafkaEventHandlers.Handlers;

public static class Convector
{
    public static ContentProcessed ToContentProcessed(this ContentProcessedEvent contentProcessedEvent)
    {
        return new ContentProcessed
        {
            Path = contentProcessedEvent.Path,
            Provider = contentProcessedEvent.Provider,
            AccompanyingCommentary = contentProcessedEvent.AccompanyingCommentary,
            ContentId = contentProcessedEvent.Id,
            IsSaved = contentProcessedEvent.IsSaved,
            IsValid = contentProcessedEvent.IsValid,
            OriginalLink = contentProcessedEvent.OriginalLink,
            Height = contentProcessedEvent.Height,
            Width = contentProcessedEvent.Width,
            Duration = contentProcessedEvent.Duration
        };
    }
}