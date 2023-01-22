namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IAbyssRepository
{
    Task<Guid?> GetUserIdAsync(long telegramUserId);
    Task<Guid> GetGroupIdAsync(long chatId);
    Task<Content?> GetContentAsync(string originalLink);
    Task<long> GetChatIdAsync(Guid groupId);
    Task AddUserToContentAsync(Guid contentId, Guid userId);
    Task SaveContentAsync(Content content);
    Task<int> GetContentCountAsync();
}