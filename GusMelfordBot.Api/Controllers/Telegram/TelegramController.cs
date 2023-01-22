using System.ComponentModel.DataAnnotations;
using GusMelfordBot.Api.Dto.Telegram;
using GusMelfordBot.Domain.Telegram;
using Microsoft.AspNetCore.Mvc;
using TBot.Telegram.Dto;

namespace GusMelfordBot.Api.Controllers.Telegram;

[ApiController]
[Route("api/telegram/update")]
public class TelegramController : Controller
{
    private readonly IUpdateService _updateService;
        
    public TelegramController(IUpdateService updateService)
    {
        _updateService = updateService;
    }

    [HttpPost]
    public async Task<IActionResult> Update([Required, FromBody] Update update)
    {
        await _updateService.ProcessUpdateAsync(update.ToDomain());
        return Ok();
    }
}