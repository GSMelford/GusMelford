namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IContentCollectorService
{
    IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter);
}