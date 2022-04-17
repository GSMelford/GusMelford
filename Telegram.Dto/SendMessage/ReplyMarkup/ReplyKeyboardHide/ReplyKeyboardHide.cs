using Newtonsoft.Json;

namespace Telegram.Dto.SendMessage.ReplyMarkup.ReplyKeyboardHide
{
    public class ReplyKeyboardHide : Abstracts.ReplyMarkup
    {
        [JsonProperty("hide_keyboard")]
        public bool HideKeyboard { get; set; }
        
        [JsonProperty("selective")]
        public bool Selective { get; set; }
    }
}