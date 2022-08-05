using GusMelfordBot.Api.Dto.ContentCollector;
using GusMelfordBot.Domain.Application.ContentCollector;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Api.Controllers;

[ApiController]
[Route("api/content-collector")]
public class ContentCollectorController : Controller
{
    private readonly IContentCollectorService _contentCollectorService;
    
    public ContentCollectorController(IContentCollectorService contentCollectorService)
    {
        _contentCollectorService = contentCollectorService;
    }

    [HttpGet("contents")]
    public IEnumerable<ContentDto> GetContents([FromQuery] ContentFilterDto filterDto)
    {
        return _contentCollectorService.GetContents(filterDto.ToDomain()).Select(x => x.ToDto());
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
    
    [HttpGet("info")]
    public async Task<ContentCollectorInfoDto> GetInfo([FromQuery] ContentFilterDto filterDto)
    {
        return (await _contentCollectorService.GetContentCollectorInfo(filterDto.ToDomain())).ToDto();
    }
    
    [HttpGet("test")]
    public string GetTest()
    {
        if (!Directory.Exists("contents"))
        {
            Directory.CreateDirectory("contents");
        }

        if (!System.IO.File.Exists(Path.Join("contents", "text.txt")))
        {
            System.IO.File.Create("text.txt");
        }
        
        return System.IO.File.ReadAllText(Path.Join("contents", "text.txt"));
    }
}