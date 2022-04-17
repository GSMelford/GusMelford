using Newtonsoft.Json;

namespace Telegram.Dto.UpdateModule
{
    public class Update
    {
        [JsonProperty("update_id")]
        public int UpdateId { get; set; }
        
        [JsonProperty("message")]
        public Message Message { get; set; }
        
        [JsonProperty("inline_query")]
        public InlineQuery InlineQuery { get; set; }
        
        [JsonProperty("chosen_inline_result")]
        public ChosenInlineResult ChosenInlineResult { get; set; }
        
        [JsonProperty("callback_query")]
        public CallbackQuery CallbackQuery { get; set; }
        
        [JsonProperty("edited_message")]
        public EditedMessage EditedMessage { get; set; }
    }
}