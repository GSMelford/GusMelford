using GusMelfordBot.Domain.Auth;

namespace GusMelfordBot.Api.Dto.Auth;

public static class Convertor
{
    public static TelegramLoginData ToDomain(this TelegramLoginDataDto telegramLoginDataDto)
    {
        return new TelegramLoginData(telegramLoginDataDto.TelegramId, telegramLoginDataDto.Password);
    }

    public static JwtDto ToDto(this Jwt jwt)
    {
        return new JwtDto
        {
            AccessToken = jwt.AccessToken,
            UserFullName = jwt.UserFullName,
            Role = jwt.Role,
            ExpiredIn = jwt.ExpiredIn,
            RefreshToken = jwt.RefreshToken
        };
    }

    public static TokensDomain ToDomain(this TokensDto tokensDto)
    {
        return new TokensDomain(tokensDto.AccessToken, tokensDto.RefreshToken);
    }
}