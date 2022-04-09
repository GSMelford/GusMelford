using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Core.Controllers;

using Interfaces;
using Microsoft.AspNetCore.Mvc;
    
[ApiController]
public class GusMelfordBotController : Controller
{
    private readonly ISystemService _systemService;
    private readonly ILogger<GusMelfordBotController> _logger;
    
    public GusMelfordBotController(
        ISystemService systemService,
        ILogger<GusMelfordBotController> logger)
    {
        _systemService = systemService;
        _logger = logger;
    }
        
    [HttpGet("info")]
    public async Task<JsonResult> GetSystemInfo()
    {
        return Json(await _systemService.GetSystemData());
    }
    
    [HttpGet("check")]
    public IActionResult CheckSystem()
    {
        _logger.LogInformation("The GusMelfordBot works good");
        return Ok();
    }
}