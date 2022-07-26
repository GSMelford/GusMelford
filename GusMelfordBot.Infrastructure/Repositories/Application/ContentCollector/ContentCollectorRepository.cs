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
            User = await _databaseContext.Set<User>().FirstAsync(x => x.Id == telegramUser.UserId),
            OriginalLink = Regex.Match(messageText, "https:///\\S*").Groups[1].Value,
            MessageId = messageId
        };
        
        await _databaseContext.AddAsync(content);
        await _databaseContext.SaveChangesAsync();
    }
    
    public async Task Update(ContentProcessed contentProcessed)
    {
        Content content = await _databaseContext.Set<Content>()
            .FirstAsync(x => x.Id == contentProcessed.ContentId);
        
        Content? sameContent = await _databaseContext.Set<Content>()
            .FirstOrDefaultAsync(x => x.OriginalLink == contentProcessed.OriginalLink);
        
        await Update(content, contentProcessed, sameContent?.Id);
    }

    private async Task Update(Content content, ContentProcessed contentProcessed, Guid? sameContentId)
    {
        content.Path = contentProcessed.Path;
        content.Provider = contentProcessed.Provider;
        content.AccompanyingCommentary = contentProcessed.AccompanyingCommentary;
        content.IsValid = contentProcessed.IsValid;
        content.IsSaved = contentProcessed.IsSaved;
        content.OriginalLink = contentProcessed.OriginalLink;
        content.SameContentId = sameContentId;
        
        _databaseContext.Update(content);
        await _databaseContext.SaveChangesAsync();
    }
}