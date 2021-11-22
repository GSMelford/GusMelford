namespace GusMelfordBot.DAL.TikTok
{
    public class Comment : DatabaseEntity
    {
        public Video Video { get; set; }
        public string Text { get; set; }
    }
}