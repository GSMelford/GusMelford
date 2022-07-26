namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IContentCollectorRepository
{
    Task Create(Guid contentId, long? chatId, long? telegramUserId, string messageText, long? messageId);
    Task Update(ContentProcessed contentProcessed);
}