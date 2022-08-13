using GusMelfordBot.Api.Dto.Auth;
using GusMelfordBot.Domain.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<JwtDto>> Login([FromBody] TelegramLoginDataDto telegramLoginDataDto)
    {
        return Ok((await _authService.Login(telegramLoginDataDto.ToDomain())).ToDto());
    }
    
    [HttpGet("video2")]
    public FileResult Get()
    {
        return PhysicalFile("/app/contents/eva.mkv", "application/octet-stream", enableRangeProcessing: true);
    }
}