using GusMelfordBot.Api.Dto.ContentCollector;
using GusMelfordBot.Api.Services.Features.Abyss;
using GusMelfordBot.Api.Services.Features.WatchTogether;
using GusMelfordBot.Domain.Application.ContentCollector;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Api.Controllers;

[ApiController]
[Route("api/content-collector")]
public class ContentCollectorController : Controller
{
    private readonly IContentCollectorService _contentCollectorService;
    private readonly IWatchTogetherRoomFactory _watchTogetherRoomFactory;
    
    public ContentCollectorController(
        IContentCollectorService contentCollectorService,
        IWatchTogetherRoomFactory watchTogetherRoomFactory)
    {
        _contentCollectorService = contentCollectorService;
        _watchTogetherRoomFactory = watchTogetherRoomFactory;
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
        return _watchTogetherRoomFactory.GetRoomByRoomCode(roomCode)?.GetContentInfo().ToDto() ?? new ContentDto();
    }
    
    [HttpPost("room")]
    public string CreateRoom([FromQuery] ContentFilterDto contentFilterDto)
    {
        return _watchTogetherRoomFactory.Create(
            _contentCollectorService.GetContents(contentFilterDto.ToDomain()).ToList());
    }
}