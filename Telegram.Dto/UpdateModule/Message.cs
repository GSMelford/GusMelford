using System.Collections.Generic;
using Newtonsoft.Json;

namespace Telegram.Dto.UpdateModule
{
    public class Message
    {
        [JsonProperty("message_id")]
        public int MessageId { get; set; }
        public int Date { get; set; }
        public int ForwardDate { get; set; }
        public int MigrateToChatId { get; set; }
        public int MigrateFromChatId { get; set; }
        public string Text { get; set; }
        public string Caption { get; set; }
        public string NewChatTitle { get; set; }
        public bool DeleteChatPhoto { get; set; }
        public bool GroupChatCreated { get; set; }
        public bool SupergroupChatCreated { get; set; }
        public bool ChannelChatCreated { get; set; }
        public User From { get; set; }
        public User NewChatMember { get; set; }
        public User LeftChatMember { get; set; }
        public User ForwardFromUser { get; set; }
        public Chat Chat { get; set; }
        public Message PinnedMessage { get; set; }
        public Message ReplyToMessage { get; set; }
        public Audio Audio { get; set; }
        public Document Document { get; set; }
        public Sticker Sticker { get; set; }
        public Video Video { get; set; }
        public Voice Voice { get; set; }
        public Contact Contact { get; set; }
        public Location Location { get; set; }
        public Venue Venue { get; set; }
        public List<MessageEntity> Entities { get; set; }
        public List<PhotoSize> PhotoSizes { get; set; }
        public List<PhotoSize> NewChatPhoto { get; set; }
        
        [JsonProperty("photo")]
        public List<Photo> Photos { get; set; }
    }
}