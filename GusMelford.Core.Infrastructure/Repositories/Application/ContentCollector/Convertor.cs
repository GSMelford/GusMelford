using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Infrastructure.Models;

namespace GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;

public static class Convertor
{
    public static ContentDomain ToDomain(this Content content)
    {
        return new ContentDomain(
            content.Id,
            $"api/content-collector/content?contentId={content.Id}",
            content.Number,
            content.Users.Select(x=> new ContentUserDomain(x.FirstName, x.LastName)).ToList(),
            content.Provider,
            content.OriginalLink,
            content.AccompanyingCommentary,
            content.Height,
            content.Width,
            content.Duration);
    }
}