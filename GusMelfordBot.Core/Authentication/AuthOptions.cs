using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GusMelfordBot.Core.Authentication;

public static class AuthOptions
{
    public const string Issuer = "GusMelfordBot";
    public const string Audience = "GusMelfordBot Site";
    private const string KEY = "jojobizarreadventure";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new (Encoding.UTF8.GetBytes(KEY));
}