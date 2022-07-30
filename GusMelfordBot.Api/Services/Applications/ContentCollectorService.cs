using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Extensions.Services.Ftp;

namespace GusMelfordBot.Api.Services.Applications;

public class ContentCollectorService : IContentCollectorService
{
    private readonly IContentCollectorRepository _contentCollectorRepository;
    private readonly IFtpServerService _ftpServerService;

    public ContentCollectorService(
        IContentCollectorRepository contentCollectorRepository,
        IFtpServerService ftpServerService)
    {
        _contentCollectorRepository = contentCollectorRepository;
        _ftpServerService = ftpServerService;
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

        MemoryStream? memoryStream = await _ftpServerService.DownloadFile(contentPath);
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