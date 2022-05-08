using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Extensions;
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
    
    [HttpGet("sendInformationPanel")]
    public async Task<JsonResult> SendInformationPanel(
        [FromQuery] string contentId)
    {
        return Json(await _contentCollectorService.SendInformationPanelAsync(contentId.ToGuid()));
    }
    
    [HttpGet("deleteInformationPanel")]
    public void DeleteInformationPanel(
        [FromQuery] string chatId, 
        [FromQuery] string messageId)
    {
        _contentCollectorService.DeleteInformationPanelAsync(chatId.ToGuid(), messageId.ToInt());
    }
}