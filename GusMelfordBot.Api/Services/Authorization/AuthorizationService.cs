using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Extensions;
using GusMelfordBot.Extensions.Exceptions;

namespace GusMelfordBot.Api.Services.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    
    public AuthorizationService(IAuthRepository authRepository, IJwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }

    public async Task<Jwt> LoginAsync(TelegramLoginData telegramLoginData)
    {
        AuthUserDomain? authUserDomain = await _authRepository.AuthenticateUser(
            telegramLoginData.TelegramId, telegramLoginData.Password.ToSha512());

        authUserDomain.IfNullThrow(new UnauthorizedException("Telegram id or password is incorrect!"));

        Jwt jwt = _jwtService.BuildJwtToken(authUserDomain);
        await _authRepository.UpdateRefreshTokenAsync(authUserDomain.Id, jwt.RefreshToken);
        return jwt;
    }

    public async Task<Jwt> RefreshTokenAsync(TokensDomain tokensDomain)
    {
        if (!_jwtService.IsValidToken(tokensDomain.AccessToken, out Guid userId)) {
            throw new UnauthorizedException("Access token is not valid");
        }
        
        AuthUserDomain authUserDomain = await _authRepository.AuthenticateUser(userId);
        if (authUserDomain.RefreshToken == tokensDomain.RefreshToken)
        {
            Jwt jwt = _jwtService.BuildJwtToken(authUserDomain);
            await _authRepository.UpdateRefreshTokenAsync(userId, jwt.RefreshToken);
            return jwt;
        }

        throw new UnauthorizedException("Refresh token is not valid");
    }
}