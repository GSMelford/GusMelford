using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Extensions;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Content = GusMelfordBot.Domain.Application.ContentCollector.Content;

namespace GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;

public class AbyssRepository : IAbyssRepository
{
    private readonly IDatabaseContext _databaseContext;

    public AbyssRepository(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Guid?> GetUserIdAsync(long telegramUserId)
    {
        return (await _databaseContext.Set<TelegramUser>()
            .FirstOrDefaultAsync(x => x.TelegramId == telegramUserId))?.UserId;
    }
    
    public async Task<Guid> GetGroupIdAsync(long chatId)
    {
        return (await _databaseContext.Set<Group>().FirstAsync(x => x.ChatId == chatId)).Id;
    }

    public async Task<Content?> GetContentAsync(string originalLink)
    {
        return (await _databaseContext.Set<Models.Content>()
            .FirstOrDefaultAsync(x => x.OriginalLink == originalLink))?
            .ToDomain();
    }
    
    public async Task<long> GetChatIdAsync(Guid groupId)
    {
        return (await _databaseContext.Set<Group>().FirstAsync(x => x.Id == groupId)).ChatId;
    }
    
    public async Task<int> GetContentCountAsync()
    {
        return await _databaseContext.Set<Content>().CountAsync();
    }
    
    public async Task SaveContentAsync(Content content)
    {
        Guid userId = content.UserIds.FirstOrDefault();
        User user = (await _databaseContext.Set<User>().FirstOrDefaultAsync(x => x.Id == userId))
            .IfNullThrow(new Exception($"User not found. User id: {userId}"));

        Models.Content? contentDal = new Models.Content
        {
            Id = content.Id,
            GroupId = content.GroupId,
            OriginalLink = content.OriginalLink,
            Users = new List<User> { user },
            Provider = content.Provider,
            MetaContent = new Models.MetaContent
            {
                IsSaved = content.MetaContent.IsSaved,
                Height = content.MetaContent.Height,
                Width = content.MetaContent.Width,
                Duration = content.MetaContent.Duration
            }
        };

        if (content.UserComments.Any())
        {
            contentDal.UserContentComments = new List<UserContentComment>
            {
                new ()
                {
                    User = user,
                    Message = content.UserComments.FirstOrDefault()?.Message!
                }
            };
        }
    }

    public async Task AddUserToContentAsync(Guid contentId, Guid userId)
    {
        Models.Content contentDal = (await _databaseContext.Set<Models.Content>()
                .FirstOrDefaultAsync(x => x.Id == contentId))
            .IfNullThrow(new Exception($"Content not found. User id: {contentId}"));
        
        User user = (await _databaseContext.Set<User>().FirstOrDefaultAsync(x => x.Id == userId))
            .IfNullThrow(new Exception($"User not found. User id: {userId}"));

        if (contentDal.Users.FirstOrDefault(x=>x.Id == userId) is not null)
        {
            contentDal.Users.Add(user);
            await _databaseContext.SaveChangesAsync();
        }
    }
}