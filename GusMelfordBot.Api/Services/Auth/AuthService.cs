using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Extensions;
using GusMelfordBot.Extensions.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace GusMelfordBot.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly IAuthRepository _authRepository;
    
    public AuthService(AppSettings appSettings, IAuthRepository authRepository)
    {
        _appSettings = appSettings;
        _authRepository = authRepository;
    }

    public async Task<Jwt> Login(TelegramLoginData telegramLoginData)
    {
        AuthUserDomain? authUserDomain = await _authRepository.AuthenticateUserByTelegramAsync(
            telegramLoginData.TelegramId, telegramLoginData.Password);

        authUserDomain.IfNullThrow(new UnauthorizedException("Telegram id or password is incorrect!"));

        return new Jwt(
            string.Join(" ", authUserDomain.FirstName, authUserDomain.LastName), 
            GetAccessToken(authUserDomain));
    }
    
    private string GetAccessToken(AuthUserDomain authUserDomain)
    {
        ClaimsIdentity claimsIdentity = GetIdentity(authUserDomain);

        DateTime utcNow = DateTime.UtcNow;
        TimeSpan lifeTime = TimeSpan.FromHours(_appSettings.AuthSettings.Lifetime);

        var jwtSecurityToken = new JwtSecurityToken(
            _appSettings.AuthSettings.Issuer,   
            _appSettings.AuthSettings.Audience,
            notBefore: utcNow,
            claims: claimsIdentity.Claims,
            expires: utcNow.Add(lifeTime),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.AuthSettings.Key)), 
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
    
    private static ClaimsIdentity GetIdentity(AuthUserDomain user)
    {
        return new ClaimsIdentity(
            new List<Claim>
            {
                new("UserId", user.Id.ToString()),
                new(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            },
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
    }
}