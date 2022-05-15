using Newtonsoft.Json;

namespace Telegram.Dto.Response
{
    public class FileResponse
    {
        [JsonProperty("ok")] 
        public string Ok { get; set; }
        
        [JsonProperty("result")] 
        public TelegramFile Result { get; set; }
    }
}