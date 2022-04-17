using System;
using Newtonsoft.Json;

namespace Telegram.Dto.UpdateModule
{
    public class EditedMessage
    {
        [JsonProperty("message_id")]
        public int MessageId { get; set; }
        
        [JsonProperty("from")]
        public User FromUser { get; set; }
        
        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("date")] 
        public int Date;
        
        [JsonProperty("edit_date")] 
        public int EditDate;
        
        [JsonProperty("text")] 
        public string Text;
    }
}