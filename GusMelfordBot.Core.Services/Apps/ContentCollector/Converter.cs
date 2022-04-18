using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector;

public static class Converter
{
    public static ContentInfo ToDomain(this DAL.Applications.ContentCollector.Content? content)
    {
        return new ContentInfo
        {
            Id = content.Id,
            AccompanyingCommentary = content.AccompanyingCommentary,
            SenderName = string.Join(" ", content.User.FirstName, content.User.LastName),
            RefererLink = content.RefererLink,
            ContentProvider = content.ContentProvider,
            ChatId = content.Chat.ChatId
        };
    }
}