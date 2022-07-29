using GusMelfordBot.Api.Dto.ContentCollector;
using GusMelfordBot.Domain.Application.ContentCollector;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Api.Controllers;

[ApiController]
[Route("content-collector")]
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
}