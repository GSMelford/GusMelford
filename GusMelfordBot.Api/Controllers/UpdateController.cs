using System.ComponentModel.DataAnnotations;
using GusMelfordBot.Api.Dto.Telegram;
using GusMelfordBot.Domain.Telegram;
using Microsoft.AspNetCore.Mvc;
using TBot.Telegram.Dto;

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
    public async Task<IActionResult> Update([Required, FromBody] Update update)
    {
        _logger.LogInformation("Update from telegram: {@Update}", update);
        
        try 
        {
            await _updateService.ProcessUpdate(update.ToDomain());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update from telegram error. UpdateId: {UpdateId}", update.UpdateId);
            return BadRequest();
        }
        
        return Ok();
    }
}