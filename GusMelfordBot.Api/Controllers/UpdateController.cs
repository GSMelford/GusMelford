using System.ComponentModel.DataAnnotations;
using GusMelfordBot.Domain.Telegram;
using Microsoft.AspNetCore.Mvc;
using TBot.Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Api.Controllers;

[ApiController]
[Route("update")]
public class UpdateController : Controller
{
    private readonly ILogger<UpdateController> _logger;
    private readonly IUpdateService _updateService;
        
    public UpdateController(
        ILogger<UpdateController> logger, 
        IUpdateService updateService)
    {
        _logger = logger;
        _updateService = updateService;
    }

    [HttpPost]
    public IActionResult Update([Required, FromBody]Update update)
    {
        _logger.LogInformation("Update. Body: {@UpdateText}", update);
        try {
            _updateService.ProcessUpdate(update);
        }
        catch (Exception e) {
            _logger.LogError(e, "ProcessUpdate error. UpdateId: {UpdateId}", update.UpdateId);
            return BadRequest();
        }
        return Ok();
    }
}