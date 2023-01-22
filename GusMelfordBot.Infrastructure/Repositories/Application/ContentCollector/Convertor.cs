using GusMelfordBot.Domain.Application.ContentCollector;
using Content = GusMelfordBot.Domain.Application.ContentCollector.Content;
using MetaContent = GusMelfordBot.Domain.Application.ContentCollector.MetaContent;

namespace GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;

public static class Convertor
{
    public static Content ToDomain(this Models.Content content)
    {
        return new Content(
            content.Id,
            content.GroupId,
            content.Users.Select(x=>x.Id).ToList(),
            content.Provider,
            content.OriginalLink,
            new MetaContent(
                content.MetaContent.IsSaved, 
                content.MetaContent.Height, 
                content.MetaContent.Width, 
                content.MetaContent.Duration,
                content.MetaContent.TelegramMessageId),
            content.UserContentComments.Select(x=>new UserComment(x.UserId, x.Message)).ToList());
    }
}