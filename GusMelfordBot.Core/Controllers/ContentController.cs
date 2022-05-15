using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Dto.Filter;
using GusMelfordBot.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Core.Controllers;

[ApiController]
[Route("app/content")]
public class ContentController : Controller
{
    private readonly IContentService _contentService;
    private readonly IContentDownloadService _contentDownloadService;
    private const string CONTENT_TYPE = "video/mp4";
    
    public ContentController(
        IContentService contentService, 
        IContentDownloadService contentDownloadService)
    {
        _contentService = contentService;
        _contentDownloadService = contentDownloadService;
    }
    
    [HttpGet("info")]
    public JsonResult GetContentInfoList([FromQuery] FilterDto filterDto)
    {
        return Json(_contentService.BuildContentInfoList(filterDto.ToDomain()));
    }
    
    [HttpGet("setViewedVideo")]
    public async Task<IActionResult> SetViewedVideo([FromQuery] string contentId)
    {
        await _contentService.SetViewedVideo(contentId.ToGuid());
        return Ok();
    }
    
    [HttpGet("cache")]
    public async Task<IActionResult> CacheContent()
    {
        await _contentService.Cache();
        return Ok();
    }
    
    [HttpGet]
    public async Task<FileStreamResult?> GetContent([FromQuery] string contentId)
    {
        MemoryStream? memoryStream = await _contentDownloadService.GetFileStreamContent(contentId.ToGuid());
        if (memoryStream is null)
        {
            return null;
        }
        
        FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, CONTENT_TYPE);
        
        HttpContext.Response.Headers.Add("Content-Length", memoryStream.Length.ToString());
        HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
        HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        HttpContext.Response.Headers.Add("Pragma", "no-cache");
        HttpContext.Response.Headers.Add("Expires", "0");
        
        return fileStreamResult;
    }
}