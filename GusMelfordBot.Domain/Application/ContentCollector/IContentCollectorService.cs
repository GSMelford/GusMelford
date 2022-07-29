namespace GusMelfordBot.Domain.Application.ContentCollector;

public interface IContentCollectorService
{
    IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter);
    Task<MemoryStream> GetContentStreamAsync(Guid contentId);
}