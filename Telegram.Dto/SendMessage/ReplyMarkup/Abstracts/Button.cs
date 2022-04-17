using Newtonsoft.Json;

namespace Telegram.Dto.SendMessage.ReplyMarkup.Abstracts
{
    public abstract class Button
    {
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }
    }
}