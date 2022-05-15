using Newtonsoft.Json;

namespace Telegram.Dto
{
    public class Photo
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }
        
        [JsonProperty("file_unique_id")]
        public string FileUniqueId { get; set; }
        
        [JsonProperty("file_size")]
        public string FileSize { get; set; }
        
        [JsonProperty("width")]
        public string Width { get; set; }
        
        [JsonProperty("height")]
        public string Height { get; set; }
    }
}