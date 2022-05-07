using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Core.Controllers;

[ApiController]
[Route("contentCollector")]
public class ContentCollectorController : Controller
{
    private readonly IContentCollectorService _contentCollectorService;
    
    public ContentCollectorController(IContentCollectorService contentCollectorService)
    {
        _contentCollectorService = contentCollectorService;
    }
}