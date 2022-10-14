namespace GusMelfordBot.Api.Settings;

public class AuthSettings
{
    public string Issuer { get; set; } = "GusMelfordBot";
    public string Audience { get; set; } = "GusMelfordBot Client";
    public string Key { get; set; }
    public int Lifetime { get; set; } = 12;
}