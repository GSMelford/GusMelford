using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Extensions.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace GusMelfordBot.Api.Services.Authorization;

public class JwtService : IJwtService
{
    private readonly AppSettings _appSettings;

    public JwtService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public Jwt BuildJwtToken(AuthUserDomain authUserDomain)
    {
        ClaimsIdentity claimsIdentity = GetIdentity(authUserDomain);

        DateTime utcNow = DateTime.UtcNow;
        TimeSpan lifeTime = TimeSpan.FromHours(_appSettings.AuthSettings!.Lifetime);
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

        return new Jwt(
            string.Join(" ", authUserDomain.FirstName, authUserDomain.LastName),
            authUserDomain.Role,
            new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
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
    
    public bool IsValidToken(string accessToken, out Guid userId)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.AuthSettings!.Key)),
            ValidateLifetime = false
        };
        
        ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler()
            .ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        bool tokenIsValid = false;
        if (securityToken is JwtSecurityToken jwtSecurityToken) {
            tokenIsValid = jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
        
        string? userIdString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        
        if (!Guid.TryParse(userIdString, out userId)) {
            throw new UnauthorizedException("Invalid Access token");
        }
        
        return tokenIsValid;
    }
}