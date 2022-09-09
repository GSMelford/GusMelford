using GusMelfordBot.Api.Dto.ContentCollector;
using GusMelfordBot.Api.Services.Applications.ContentCollector;
using GusMelfordBot.Domain.Application.ContentCollector;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Api.Controllers;

[ApiController]
[Route("api/content-collector")]
public class ContentCollectorController : Controller
{
    private readonly IContentCollectorService _contentCollectorService;
    private readonly IContentCollectorRoomFactory _contentCollectorRoomFactory;
    private readonly IContentCollectorRepository _contentCollectorRepository;
    
    public ContentCollectorController(
        IContentCollectorService contentCollectorService,
        IContentCollectorRoomFactory contentCollectorRoomFactory, 
        IContentCollectorRepository contentCollectorRepository)
    {
        _contentCollectorService = contentCollectorService;
        _contentCollectorRoomFactory = contentCollectorRoomFactory;
        _contentCollectorRepository = contentCollectorRepository;
    }
    
    [HttpGet("contents")]
    public IEnumerable<ContentDto> GetContents([FromQuery] ContentFilterDto filterDto)
    {
        return _contentCollectorService.GetContents(filterDto.ToDomain()).Select(x => x.ToDto());
    }
    
    [HttpGet("info")]
    public async Task<ContentCollectorInfoDto> GetInfo([FromQuery] ContentFilterDto filterDto)
    {
        return (await _contentCollectorService.GetContentCollectorInfo(filterDto.ToDomain())).ToDto();
    }
    
    [HttpGet("content")]
    public async Task<FileStreamResult?> GetContent([FromQuery] Guid contentId)
    {
        MemoryStream memoryStream = await _contentCollectorService.GetContentStreamAsync(contentId);
        FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "video/mp4");
        
        HttpContext.Response.Headers.Add("Content-Length", memoryStream.Length.ToString());
        HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
        HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        HttpContext.Response.Headers.Add("Pragma", "no-cache");
        HttpContext.Response.Headers.Add("Expires", "0");
        
        return fileStreamResult;
    }

    [HttpGet("room/content/info")]
    public ContentDto GetContentInfo([FromQuery] string roomCode)
    {
        return _contentCollectorRoomFactory.FindRoomByRoomCode(roomCode)?.GetContentInfo().ToDto() ?? new ContentDto();
    }
    
    [HttpPost("room")]
    public string CreateRoom([FromQuery] ContentFilterDto contentFilterDto)
    {
        return _contentCollectorRoomFactory.Create(
            _contentCollectorService.GetContents(contentFilterDto.ToDomain()).ToList());
    }

    [HttpPatch("mark-content-as-viewed")]
    public async Task MarkContentAsViewed([FromQuery] Guid contentId)
    {
        await _contentCollectorRepository.MarkContentAsViewed(contentId);
    }
}