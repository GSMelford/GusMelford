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
            Id = contentDomain.Id,
            ContentPath = contentDomain.ContentPath,
            Users = contentDomain.Users.Select(x=>new UserDto
            {
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList(),
            OriginalLink = contentDomain.OriginalLink,
            Height = contentDomain.Height,
            Width = contentDomain.Width,
            Duration = contentDomain.Duration,
            Number = contentDomain.Number,
            AccompanyingCommentary = contentDomain.AccompanyingCommentary,
            Provider = contentDomain.Provider
        };
    }
    
    public static ContentCollectorInfoDto ToDto(this ContentCollectorInfo contentCollectorInfo)
    {
        return new ContentCollectorInfoDto
        {
            Duration = contentCollectorInfo.Duration,
            ContentCount = contentCollectorInfo.ContentCount
        };
    }
}