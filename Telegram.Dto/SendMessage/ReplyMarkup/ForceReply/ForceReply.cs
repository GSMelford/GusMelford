using Newtonsoft.Json;

namespace Telegram.Dto.SendMessage.ReplyMarkup.ForceReply
{
    public class ForceReply : Abstracts.ReplyMarkup
    {
        [JsonProperty("force_reply")]
        public bool HideKeyboard { get; set; }
        
        [JsonProperty("selective")]
        public bool Selective { get; set; }
    }
}