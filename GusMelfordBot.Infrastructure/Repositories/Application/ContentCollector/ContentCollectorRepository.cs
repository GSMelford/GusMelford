using System.Text.RegularExpressions;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;

public class ContentCollectorRepository : IContentCollectorRepository
{
    private readonly ILogger<IContentCollectorRepository> _logger;
    private readonly IDatabaseContext _databaseContext;
    
    public ContentCollectorRepository(ILogger<IContentCollectorRepository> logger, IDatabaseContext databaseContext)
    {
        _logger = logger;
        _databaseContext = databaseContext;
    }

    public async Task<bool> SaveNew(Guid contentId, long? chatId, long? telegramUserId, string messageText, long? messageId)
    {
        User? user = (await _databaseContext.Set<TelegramUser>()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TelegramId == telegramUserId))?.User;

        if (user is null) {
            _logger.LogWarning("ContentId: {ContentId}. User not found or not registered. " +
                               "TelegramUserId {TelegramUserId}", contentId, telegramUserId);
            return false;
        }
        
        TelegramChat? telegramChat = 
            await _databaseContext.Set<TelegramChat>().FirstOrDefaultAsync(x => x.ChatId == chatId);
        
        if (telegramChat is null) {
            _logger.LogWarning("ContentId: {ContentId}. Chat not found. ChatId: {ChatId}", contentId, chatId);
            return false;
        }
        
        Content content = new Content
        {
            Id = contentId,
            Chat = telegramChat,
            OriginalLink = Regex.Match(messageText, "https://\\S*").Groups[0].Value,
            MessageId = messageId
        };
        
        content.Users.Add(user);
        await _databaseContext.AddAsync(content);
        await _databaseContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<Guid> Update(ContentProcessed contentProcessed)
    {
        List<Content> contents = await _databaseContext.Set<Content>()
            .Include(x=>x.Users)
            .Where(x => x.OriginalLink == contentProcessed.OriginalLink || x.Id == contentProcessed.ContentId)
            .OrderBy(x=>x.CreatedOn)
            .ToListAsync();

        Content content = new Content();
        if (IsDuplicateContent(contents))
        {
            content = UpdateFirstContent(contents);
            for (int i = 1; i < contents.Count; i++)
            {
                _databaseContext.Remove(contents[i]);
            }
        }
        else if (contents.Count > 0)
        {
            content = contents.First();
        }
        
        content.Path = contentProcessed.Path;
        content.Provider = contentProcessed.Provider;
        content.AccompanyingCommentary = contentProcessed.AccompanyingCommentary;
        content.IsValid = contentProcessed.IsValid;
        content.IsSaved = contentProcessed.IsSaved;
        content.OriginalLink = contentProcessed.OriginalLink;
        content.Height = contentProcessed.Height;
        content.Width = contentProcessed.Width;
        content.Duration = contentProcessed.Duration;
        
        _databaseContext.Update(content);
        await _databaseContext.SaveChangesAsync();
        return content.Id;
    }

    private bool IsDuplicateContent(List<Content> contents)
    {
        return contents.Count > 1;
    }

    private Content UpdateFirstContent(List<Content> contents)
    {
        Content firstContent = contents.First();
        for (int i = 1; i < contents.Count; i++)
        {
            User? user = contents[i].Users.FirstOrDefault();
            if (user is null) {
                continue;
            }
            
            firstContent.Users.Add(user);
        }

        List<string> accompanyingCommentaries = 
            (firstContent.AccompanyingCommentary?.Split(";") ?? Array.Empty<string>()).ToList();
        firstContent.AccompanyingCommentary = 
            string.Join(";", accompanyingCommentaries.Concat(contents.Select(x => x.AccompanyingCommentary)));

        return firstContent;
    }

    public IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter)
    {
        return _databaseContext.Set<Content>()
            .Include(x => x.Users)
            .Where(x => x.IsViewed == contentFilter.IsViewed && x.IsSaved && x.IsValid == true)
            .OrderBy(x=> x.Number)
            .Select(x=>x.ToDomain());
    }

    public async Task<string?> GetContentPath(Guid contentId)
    {
        return (await _databaseContext.Set<Content>().FirstOrDefaultAsync(x => x.Id == contentId))?.Path;
    }

    public async Task<string> GetVideoCaption(Guid contentId)
    {
        Content? content = await _databaseContext.Set<Content>()
            .Include(x => x.Chat)
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == contentId);

        if (content is null) {
            return string.Empty;
        }
        
        User lastUser = content.Users.Last();

        string caption = $"🐤 Content №{content.Number}" +
                         $"🤖 {contentId}\n" +
                         $"👉 {lastUser.FirstName} {lastUser.LastName}\n" +
                         $"{content.OriginalLink}\n";

        if (!string.IsNullOrEmpty(content.AccompanyingCommentary)) {
            caption += $"🧐 {content.AccompanyingCommentary}";
        }

        return caption;
    }

    public async Task<long?> GetChatId(Guid contentId)
    {
        return (await _databaseContext.Set<Content>()
            .Include(x=>x.Chat)
            .FirstOrDefaultAsync(x => x.Id == contentId))?.Chat.ChatId;
    }
    
    public async Task<ContentCollectorInfo> GetContentCollectorInfo(ContentFilter contentFilter)
    {
        List<Content> contents = await _databaseContext.Set<Content>()
            .Include(x => x.Users)
            .Where(x => x.IsViewed == contentFilter.IsViewed && x.IsSaved && x.IsValid == true)
            .ToListAsync();
        
        return new ContentCollectorInfo(contents.Count, contents.Sum(x => x.Duration)!.Value);
    }
}