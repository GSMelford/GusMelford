using System.Collections.Generic;
using Telegram.Dto.UpdateModule;

namespace Telegram.Dto.Response
{
    using Newtonsoft.Json;
    public class UpdateResponse
    {
        [JsonProperty("ok")] 
        public string Ok { get; set; }
        
        [JsonProperty("result")] 
        public List<Update> Result { get; set; }
    }
}