using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.DAL.Applications.ContentCollector;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector;

public static class Converter
{
    public static ContentInfoDomain ToDomain(this Content? content)
    {
        return new ContentInfoDomain
        {
            Id = content?.Id,
            AccompanyingCommentary = content?.AccompanyingCommentary,
            SenderName = string.Join(" ", content?.User.FirstName, content?.User.LastName),
            RefererLink = content?.RefererLink,
            ContentProvider = content?.ContentProvider,
            ChatId = content?.Chat.ChatId,
            Description = content?.Description
        };
    }
}