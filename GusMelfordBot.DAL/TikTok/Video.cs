namespace GusMelfordBot.DAL.TikTok
{
    public class Video : DatabaseEntity
    {
        public User User { get; set; }
        public string SentLink { get; set; }
        public string RefererLink { get; set; }
        public bool IsViewed { get; set; }
        public bool? IsValid { get; set; }
    }
}