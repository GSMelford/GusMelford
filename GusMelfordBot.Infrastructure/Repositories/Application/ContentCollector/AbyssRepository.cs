using GusMelfordBot.Domain;
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
                .Include(x=>x.MetaContent)
                .Include(x=>x.Users)
            .FirstOrDefaultAsync(x => x.OriginalLink == originalLink))?
            .ToDomain();
    }
    
    public async Task<long> GetChatIdAsync(Guid groupId)
    {
        return (await _databaseContext.Set<Group>().FirstAsync(x => x.Id == groupId)).ChatId;
    }
    
    public async Task<int> GetContentCountAsync()
    {
        return await _databaseContext.Set<Models.Content>().CountAsync();
    }
    
    public async Task SaveContentAsync(Content content)
    {
        Guid userId = content.UserIds.FirstOrDefault();
        User user = (await _databaseContext.Set<User>().FirstOrDefaultAsync(x => x.Id == userId))
            .IfNullThrow(new Exception($"User not found. User id: {userId}"));

        Models.Content contentDal = new Models.Content
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

        if (content.UserComments.Any()) {
            contentDal.UserContentComments = new List<UserContentComment> {
                new () {
                    User = user,
                    Message = content.UserComments.FirstOrDefault()?.Message!
                }
            };
        }

        await _databaseContext.AddAsync(contentDal);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task AddUserToContentAsync(Guid contentId, Content content)
    {
        Guid newUserId = content.UserIds.First();
        
        Models.Content contentDal = (await _databaseContext.Set<Models.Content>()
                .Include(x=>x.UserContentComments)
                .FirstOrDefaultAsync(x => x.Id == contentId))
            .IfNullThrow(new Exception($"Content not found. User id: {contentId}"));
        
        User user = (await _databaseContext.Set<User>().FirstOrDefaultAsync(x => x.Id == newUserId))
            .IfNullThrow(new Exception($"User not found. User id: {newUserId}"));

        if (contentDal.Users.FirstOrDefault(x=>x.Id == newUserId) is null)
        {
            contentDal.Users.Add(user);
        }

        if (content.UserComments.Any())
        {
            string userComment = content.UserComments.First().Message;
            contentDal.UserContentComments.Add(new UserContentComment
            {
                Message = userComment,
                User = user
            });
        }
        
        await _databaseContext.SaveChangesAsync();
    }

    public async Task AddAttemptMessageAsync(AttemptContent attemptContent)
    {
        Guid featureId = (await _databaseContext.Set<Feature>()
            .FirstOrDefaultAsync(x => x.Name == Constants.Feature.Abyss))!.Id;

        await _databaseContext.AddAsync(attemptContent.ToDal(featureId, attemptContent.SessionId));
        await _databaseContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<AttemptContent>> GetAttemptContentAsync(int take)
    {
        const int maxAttempt = 5;
        int localTake = take;
        
        List<AttemptMessage> attemptMessages = new List<AttemptMessage>();

        for (int i = maxAttempt; i > 0; i--)
        {
            var temp = i;
            attemptMessages.AddRange(_databaseContext
                .Set<AttemptMessage>()
                .Where(x=>x.Attempt == temp).Take(localTake));

            if (attemptMessages.Count == take) {
                break;
            }
            
            localTake -= attemptMessages.Count;
        }

        var attemptContents = attemptMessages.Select(x => x.ToDomain());
        _databaseContext.RemoveRange(attemptMessages);
        await _databaseContext.SaveChangesAsync();
        return attemptContents;
    }

    public async Task<bool> RegisterGroupAsAbyssAsync(long chatId)
    {
        Group? group = await _databaseContext.Set<Group>()
            .Include(x => x.Feature)
            .FirstOrDefaultAsync(x => x.ChatId == chatId && x.Feature.Name == Constants.Feature.Abyss);

        if (group is not null) {
            return false;
        }
        
        Feature feature = await _databaseContext.Set<Feature>().FirstAsync(x => x.Name == Constants.Feature.Abyss);
        await _databaseContext.AddAsync(new Group
        {
            Feature = feature,
            ChatId = chatId
        });

       await _databaseContext.SaveChangesAsync();
       return true;
    }

    public async Task<string> GetFunnyPhraseAsync(Guid userId)
    {
        bool isPersonalPhrase = new Random().Next(0, 10) < 4;

        var funnyPhrase = isPersonalPhrase
            ? await _databaseContext.Set<FunnyPhrase>().Where(x => x.UserId == userId).ToListAsync()
            : await _databaseContext.Set<FunnyPhrase>().ToListAsync();
        
        return funnyPhrase[new Random().Next(0, funnyPhrase.Count)].Text;
    }

    public async Task SaveTelegramMessageIdAsync(Guid contentId, int messageId)
    {
        Models.Content content = await _databaseContext.Set<Models.Content>()
            .Include(x=>x.MetaContent)
            .FirstAsync(x => x.Id == contentId);

        content.MetaContent.TelegramMessageId = messageId;
        
        _databaseContext.Update(content);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task<string> GetUserNameAsync(Guid userId)
    {
        var user = await _databaseContext.Set<User>().FirstAsync(x => x.Id == userId);
        return $"{user.FirstName} {user.LastName}";
    }
    
    public async Task<string> GetTelegramUserNameAsync(Guid userId)
    {
        var telegramUser = await _databaseContext.Set<TelegramUser>().FirstAsync(x => x.UserId == userId);
        return $"@{telegramUser.UserName}";
    }
}