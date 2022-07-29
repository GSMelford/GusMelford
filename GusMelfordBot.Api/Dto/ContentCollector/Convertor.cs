using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Dto.ContentCollector;

public static class Convertor
{
    public static ContentFilter ToDomain(this ContentFilterDto contentFilterDto)
    {
        return new ContentFilter(contentFilterDto.IsViewed);
    }
    
    public static ContentDto ToDto(this ContentDomain contentDomain)
    {
        return new ContentDto
        {
            User = new UserDto
            {
                FirstName = contentDomain.User.FirstName,
                LastName = contentDomain.User.LastName
            },
            OriginalLink = contentDomain.OriginalLink,
            Height = contentDomain.Height,
            Width = contentDomain.Width,
            Duration = contentDomain.Duration,
            Number = contentDomain.Number,
            AccompanyingCommentary = contentDomain.AccompanyingCommentary,
            Provider = contentDomain.Provider
        };
    }
}