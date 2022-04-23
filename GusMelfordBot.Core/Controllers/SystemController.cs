using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Dto.System;
using GusMelfordBot.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Core.Controllers;

[ApiController]
[Route("system")]
public class SystemController : Controller
{
    private readonly ISystemService _systemService;
    private readonly CommonSettings _commonSettings;
    
    public SystemController(
        ISystemService systemService, CommonSettings commonSettings)
    {
        _systemService = systemService;
        _commonSettings = commonSettings;
    }

    [HttpGet("info")]
    public JsonResult GetSystemInfo()
    {
        return Json(_systemService.BuildSystemInfo(_commonSettings.Name, _commonSettings.Version).ToDto());
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CredentialsDto credentialsDto)
    {
        if (await _systemService.Login(credentialsDto.ToDomain()))
        {
            return Ok();
        }

        return Unauthorized();
    }
    
    [HttpPost("addUser")]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        await _systemService.AddUser(userDto.ToDomain());
        return Ok();
    }
}