using Newtonsoft.Json;
using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;

namespace Telegram.Dto.SendMessage.ReplyMarkup.ReplyKeyboardMarkup
{
    public class ReplyKeyboardMarkup : Keyboard<ReplyKeyboardButton>
    {
        protected override int MaxButtons { get; set; } = 8;
        
        [JsonProperty("keyboard")]
        public override ReplyKeyboardButton[][] Buttons { get; set; }

        [JsonProperty("resize_keyboard")]
        public bool ResizeKeyboard { get; set; }
        
        [JsonProperty("one_time_keyboard")]
        public bool OneTimeKeyboard { get; set; }
        
        [JsonProperty("selective")]
        public bool Selective { get; set; }
        
        public new void AddRaw(KeyboardRaw<ReplyKeyboardButton> keyboardRaw)
        {
            base.AddRaw(keyboardRaw);
        }
    }
}