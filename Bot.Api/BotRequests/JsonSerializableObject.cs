using Newtonsoft.Json;

namespace Bot.Api.BotRequests
{
    public class JsonSerializableObject
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}