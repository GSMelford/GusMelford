namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IAbyssRepository
{
    Task<Guid?> GetUserIdAsync(long telegramUserId);
    Task<Guid> GetGroupIdAsync(long chatId);
    Task<Content?> GetContentAsync(string originalLink);
    Task<long> GetChatIdAsync(Guid groupId);
    Task AddUserToContentAsync(Guid contentId, Content content);
    Task SaveContentAsync(Content content);
    Task<int> GetContentCountAsync();
    Task AddAttemptMessageAsync(AttemptContent attemptContent);
    Task<IEnumerable<AttemptContent>> GetAttemptContentAsync(int take, int maxAttempt);
    Task<bool> RegisterGroupAsAbyssAsync(long chatId);
    Task<string> GetFunnyPhraseAsync(Guid userId);
    Task SaveTelegramMessageIdAsync(Guid contentId, int messageId);
    Task<string> GetUserNameAsync(Guid userId);
    Task<string> GetTelegramUserNameAsync(Guid userId);
}