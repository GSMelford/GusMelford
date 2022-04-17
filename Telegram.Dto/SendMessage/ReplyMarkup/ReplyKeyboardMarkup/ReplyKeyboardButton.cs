using Newtonsoft.Json;
using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;

namespace Telegram.Dto.SendMessage.ReplyMarkup.ReplyKeyboardMarkup
{
    public class ReplyKeyboardButton : Button
    {
        [JsonProperty("request_contact")]
        public bool RequestContact { get; set; }
        
        [JsonProperty("request_location")]
        public bool RequestLocation { get; set; }
    }
}