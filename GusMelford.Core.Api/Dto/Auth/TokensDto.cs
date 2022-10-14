namespace GusMelfordBot.Api.Dto.Auth;

public class TokensDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}