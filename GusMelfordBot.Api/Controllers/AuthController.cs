using GusMelfordBot.Api.Dto.Auth;
using GusMelfordBot.Domain.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GusMelfordBot.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    
    public AuthController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<JwtDto>> Login([FromBody] TelegramLoginDataDto telegramLoginDataDto)
    {
        return Ok((await _authorizationService.LoginAsync(telegramLoginDataDto.ToDomain())).ToDto());
    }
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult<JwtDto>> RefreshToken([FromBody] TokensDto tokensDto)
    {
        return (await _authorizationService.RefreshTokenAsync(tokensDto.ToDomain())).ToDto();
    }
}