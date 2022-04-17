using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Extensions;

public static class RequestExtension
{
    public static async Task<JToken> GetJTokenAsyncOrEmpty(this HttpResponseMessage httpResponseMessage)
    {
        JToken token;
            
        try
        {
            token = JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
        }
        catch
        {
            token = new JObject();
        }
            
        return token;
    }

    public static string BuildQuery(this Dictionary<string, string>? parameters)
    {
        return "?" + string.Join("&", parameters?.Select(pair => $"{pair.Key}={pair.Value}") ?? Array.Empty<string>());
    }
}