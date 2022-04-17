using GusMelfordBot.Core.Extensions;

namespace GusMelfordBot.Core.Dto.Filter;

public static class Convert
{
    public static Domain.Apps.ContentCollector.Content.Filter ToDomain(this FilterDto filterDto)
    {
        return new Domain.Apps.ContentCollector.Content.Filter
        {
            ChatId = filterDto.ChatId.ToGuid(),
            ContentProviders = filterDto.ContentProviders.ToList(),
            IsNotViewed = filterDto.IsNotViewed.ToBool(),
            SinceDateTime = filterDto.SinceDateTime.ToDateTime(),
            UntilDateTime = filterDto.UntilDateTime.ToDateTime()
        };
    }
}