using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Extensions.Services.DataLake;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public class ContentCollectorService : IContentCollectorService
{
    private readonly IDataLakeService _dataLakeService;

    public ContentCollectorService(
        IDataLakeService dataLakeService)
    {
        _dataLakeService = dataLakeService;
    }

    public IEnumerable<ContentDomain> GetContents(ContentFilter contentFilter)
    {
        return null;
    }
    
    public async Task<MemoryStream> GetContentStreamAsync(Guid contentId)
    {
        string? contentPath = string.Empty;
        
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
        return null;
    }
}