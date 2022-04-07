using System;

namespace GusMelfordBot.Core.Controllers;

using System.Threading.Tasks;
using Services.Update;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    public async Task<IActionResult> Update([FromBody] object update)
    {
        string updateContent = update?.ToString();
        _logger.LogInformation("New update: {Update}", updateContent);
        
        if (string.IsNullOrEmpty(updateContent))
        {
            return BadRequest();
        }
        
        try
        {
            if (!await _updateService.ProcessUpdate(updateContent))
            {
                return BadRequest();
            }
        }
        catch (Exception exception)
        {
            _logger.LogError("Update error: {Exception}", exception);
            return BadRequest();
        }

        return Ok();
    }
}