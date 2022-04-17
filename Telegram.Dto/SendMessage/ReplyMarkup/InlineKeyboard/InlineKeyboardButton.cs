using Newtonsoft.Json;
using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;

namespace Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard
{
    public class InlineKeyboardButton : Button
    {
        [JsonProperty("callback_data", Required = Required.Always)]
        public string CallbackData { get; set; }
        
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Url { get; set; }
        
        [JsonProperty("switch_inline_query", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SwitchInlineQuery { get; set; }
        
        [JsonProperty("switch_inline_query_current_chat", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SwitchInlineQueryCurrentChat { get; set; }
        
        [JsonProperty("callback_game", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CallbackGame { get; set; }
    }
}