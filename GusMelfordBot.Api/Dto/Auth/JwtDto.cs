namespace GusMelfordBot.Api.Dto.Auth;

public class JwtDto
{
    public string UserFullName { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public DateTime ExpiredIn { get; set; }
    public string RefreshToken { get; set; }
}