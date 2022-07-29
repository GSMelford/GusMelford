using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications;

public class ContentCollectorService : IContentCollectorService
{
    private readonly IContentCollectorRepository _contentCollectorRepository;

    public ContentCollectorService(IContentCollectorRepository contentCollectorRepository)
    {
        _contentCollectorRepository = contentCollectorRepository;
    }

    public IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter)
    {
        return _contentCollectorRepository.GetContents(contentFilter);
    }
}