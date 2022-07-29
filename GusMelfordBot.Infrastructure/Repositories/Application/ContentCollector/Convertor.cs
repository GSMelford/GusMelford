using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Infrastructure.Models;

namespace GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;

public static class Convertor
{
    public static ContentDomain ToDomain(this Content content)
    {
        return new ContentDomain(
            content.Number,
            new UserDomain(content.User.FirstName, content.User.LastName),
            content.Provider,
            content.OriginalLink,
            content.AccompanyingCommentary,
            content.Height,
            content.Width,
            content.Duration);
    }
}