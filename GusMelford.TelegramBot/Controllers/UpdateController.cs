using GusMelford.TelegramBot.Domain.Update.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TBot.Telegram.Dto;

namespace GusMelford.TelegramBot.Controllers;

[ApiController]
[Route("api/update")]
public class UpdateController : ControllerBase
{
    private IUpdateService _updateService;

    public UpdateController(IUpdateService updateService)
    {
        _updateService = updateService;
    }

    public void GetUpdate([FromBody] Update update)
    {
        
    }
}