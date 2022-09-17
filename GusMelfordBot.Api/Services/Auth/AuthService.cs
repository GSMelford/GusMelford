using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

    public async Task<Jwt> LoginAsync(TelegramLoginData telegramLoginData)
    {
        AuthUserDomain? authUserDomain = await _authRepository.AuthenticateUser(
            telegramLoginData.TelegramId, telegramLoginData.Password.ToSha512());

        authUserDomain.IfNullThrow(new UnauthorizedException("Telegram id or password is incorrect!"));

        Jwt jwt = BuildJwtToken(authUserDomain);
        await _authRepository.UpdateRefreshTokenAsync(authUserDomain.Id, jwt.RefreshToken);
        return jwt;
    }
    
    private Jwt BuildJwtToken(AuthUserDomain authUserDomain)
    {
        ClaimsIdentity claimsIdentity = GetIdentity(authUserDomain);

        DateTime utcNow = DateTime.UtcNow;
        TimeSpan lifeTime = TimeSpan.FromHours(_appSettings.AuthSettings.Lifetime);
        DateTime expires = utcNow.Add(lifeTime);

        var jwtSecurityToken = new JwtSecurityToken(
            _appSettings.AuthSettings.Issuer,   
            _appSettings.AuthSettings.Audience,
            notBefore: utcNow,
            claims: claimsIdentity.Claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.AuthSettings.Key)), 
                SecurityAlgorithms.HmacSha256));

        string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new Jwt(
            string.Join(" ", authUserDomain.FirstName, authUserDomain.LastName),
            authUserDomain.Role,
            accessToken,
            expires,
            GenerateRefreshToken(128));
    }
    
    private static string GenerateRefreshToken(int size)
    {
        var randomNumber = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
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
    
    public async Task<Jwt> RefreshTokenAsync(TokensDomain tokensDomain)
    {
        ClaimsPrincipal claimsPrincipal = ClaimsPrincipalFromExpiredToken(tokensDomain.AccessToken);
        string? userIdString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        
        if (!Guid.TryParse(userIdString, out Guid userId)) {
            throw new UnauthorizedException("Invalid Access token");
        }
        
        AuthUserDomain authUserDomain = await _authRepository.AuthenticateUser(userId);
        if (authUserDomain.RefreshToken == tokensDomain.RefreshToken)
        {
            Jwt jwt = BuildJwtToken(authUserDomain);
            await _authRepository.UpdateRefreshTokenAsync(userId, jwt.RefreshToken);
            return jwt;
        }

        throw new UnauthorizedException("Refresh token is not valid");
    }
    
    private ClaimsPrincipal ClaimsPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.AuthSettings.Key)),
            ValidateLifetime = false
        };
        
        ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler()
            .ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        bool tokenIsValid = false;
        if (securityToken is JwtSecurityToken jwtSecurityToken) {
            tokenIsValid = jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
        
        return tokenIsValid
            ? claimsPrincipal 
            : throw new SecurityTokenException("Invalid token");
    }
}