using GusMelfordBot.DataLake;
using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications;

public class ContentCollectorService : IContentCollectorService
{
    private readonly IContentCollectorRepository _contentCollectorRepository;
    private readonly IDataLakeService _dataLakeService;

    public ContentCollectorService(
        IContentCollectorRepository contentCollectorRepository,
        IDataLakeService dataLakeService)
    {
        _contentCollectorRepository = contentCollectorRepository;
        _dataLakeService = dataLakeService;
    }

    public IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter)
    {
        return _contentCollectorRepository.GetContents(contentFilter);
    }
    
    public async Task<MemoryStream> GetContentStreamAsync(Guid contentId)
    {
        string? contentPath = await _contentCollectorRepository.GetContentPath(contentId);
        
        if (string.IsNullOrEmpty(contentPath)) {
            throw new Exception("Not video path");
        }

        MemoryStream? memoryStream = new MemoryStream(await _dataLakeService.ReadFileAsync(contentPath));
        if (memoryStream is null) {
            throw new Exception("Video not found");
        }
        
        return memoryStream;
    }

    public Task<ContentCollectorInfo> GetContentCollectorInfo(ContentFilter contentFilter)
    {
        return _contentCollectorRepository.GetContentCollectorInfo(contentFilter);
    }
}