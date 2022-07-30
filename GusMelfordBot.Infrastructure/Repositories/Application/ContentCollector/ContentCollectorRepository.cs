using System.Text.RegularExpressions;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;

public class ContentCollectorRepository : IContentCollectorRepository
{
    private readonly IDatabaseContext _databaseContext;
    
    public ContentCollectorRepository(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task Create(Guid contentId, long? chatId, long? telegramUserId, string messageText, long? messageId)
    {
        TelegramUser telegramUser = await _databaseContext.Set<TelegramUser>()
            .FirstAsync(x => x.TelegramId == telegramUserId);
        
        Content content = new Content
        {
            Id = contentId,
            Chat = await _databaseContext.Set<TelegramChat>().FirstAsync(x => x.ChatId == chatId),
            OriginalLink = Regex.Match(messageText, "https://\\S*").Groups[0].Value,
            MessageId = messageId
        };
        
        content.Users.Add(await _databaseContext.Set<User>().FirstAsync(x => x.Id == telegramUser.UserId));
        await _databaseContext.AddAsync(content);
        await _databaseContext.SaveChangesAsync();
    }
    
    public async Task Update(ContentProcessed contentProcessed)
    {
        Content content = await _databaseContext.Set<Content>()
            .Include(x=>x.Users)
            .FirstAsync(x => x.Id == contentProcessed.ContentId);
        
        Content? sameContent = await _databaseContext.Set<Content>()
            .Include(x=>x.Users)
            .FirstOrDefaultAsync(x => x.OriginalLink == contentProcessed.OriginalLink);

        if (sameContent is not null)
        {
            if (!string.IsNullOrEmpty(contentProcessed.AccompanyingCommentary))
            {
                string? updateAccompanyingCommentary = sameContent.AccompanyingCommentary;
                if (sameContent.Users.Count == 1 && !string.IsNullOrEmpty(updateAccompanyingCommentary))
                {
                    updateAccompanyingCommentary =
                        $"{sameContent.Users.First().FirstName} {sameContent.Users.First().FirstName}: {sameContent.AccompanyingCommentary}";
                }

                updateAccompanyingCommentary +=
                    $"{content.Users.First().FirstName} {content.Users.First().FirstName}: {contentProcessed.AccompanyingCommentary}";
                sameContent.AccompanyingCommentary = updateAccompanyingCommentary;
            }
            
            sameContent.Users.Add(content.Users.FirstOrDefault()!);
            _databaseContext.Update(sameContent);
            _databaseContext.Remove(content);
            await _databaseContext.SaveChangesAsync();
            return;
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
    }

    public IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter)
    {
        return _databaseContext.Set<Content>()
            .Include(x => x.Users)
            .Where(x => x.IsViewed == contentFilter.IsViewed && x.IsSaved && x.IsValid == true)
            .Select(x=>x.ToDomain());
    }

    public async Task<string?> GetContentPath(Guid contentId)
    {
        return (await _databaseContext.Set<Content>().FirstOrDefaultAsync(x => x.Id == contentId))?.Path;
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