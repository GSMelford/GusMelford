using Newtonsoft.Json;
using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;

namespace Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard
{
    public class InlineKeyboardMarkup : Keyboard<InlineKeyboardButton>
    {
        protected override int MaxButtons => 8;
        
        [JsonProperty("inline_keyboard")]
        public override InlineKeyboardButton[][] Buttons { get; set; }
        
        public new void AddRaw(KeyboardRaw<InlineKeyboardButton> keyboardRaw)
        {
            base.AddRaw(keyboardRaw);
        }
    }
}