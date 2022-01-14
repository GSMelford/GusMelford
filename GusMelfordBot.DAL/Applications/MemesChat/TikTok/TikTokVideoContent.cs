namespace GusMelfordBot.DAL.Applications.MemesChat.TikTok
{
    public class TikTokVideoContent : DatabaseEntity
    {
        public User User { get; set; }
        public string SentLink { get; set; }
        public string RefererLink { get; set; }
        public string Description { get; set; }
        public string AccompanyingCommentary { get; set; }
        public int MessageId { get; set; }
        public bool IsViewed { get; set; }
        public bool? IsValid { get; set; }
    }
}