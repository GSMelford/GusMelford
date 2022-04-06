using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Services.GusMelfordBot;

public static class GusMelfordBotUtils
{
    public static async Task<string> GetMessageIdFromResponse(HttpResponseMessage httpResponseMessage)
    {
        try
        {
            JToken token = JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
            return token["result"]?["message_id"]?.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }
}