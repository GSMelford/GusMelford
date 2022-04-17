using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders.TikTok;

public static class TikTokServiceHelper
{
    public const string TikTok = "tiktok";
    
    private static readonly List<string> EmojiList = new()
    {
        "😳", "🥵", "😂", "😘", "❤️", "😜", "💋",
        "😒", "😍", "🙊", "😆", "😋", "😍", "🙈", 
        "😖", "🥸", "😎", "🥺", "🥳"
    };
    
    public static string GetProcessMessage(Message message)
    {
        return $"⚙️{message.From.FirstName} {message.From.LastName} sent\n" +
               $"{message.Text}\n" +
               "This tiktok is being processed";
    }

    public static string GetEditedMessage(
        DAL.Applications.ContentCollector.Content tikTokContent, 
        int count, 
        string accompanyingCommentary)
    {
        string text = $"{GetRandomEmoji()} " +
                      $"{tikTokContent.User.FirstName} {tikTokContent.User.LastName} sent meme №{count + 1}\n" +
                      $"{tikTokContent.RefererLink}";

        if (!string.IsNullOrEmpty(accompanyingCommentary))
        {
            text += $"\n{accompanyingCommentary}";
        }

        return text;
    }

    public static string GetEditedMessageAboutExist(Message message)
    {
        return $"{GetRandomEmoji()} " +
               $"{message.From.FirstName} {message.From.LastName} sent meme which existed\n" +
               $"{message.Text}";
    }
        
    public static string GetEditedMessageWhetException(Message message)
    {
        return $"😐 {message.From.FirstName} {message.From.LastName} sent meme and got an error\n" +
               $"{message.Text}";
    }

    private static string GetRandomEmoji()
    {
        return EmojiList[new Random().Next(0, EmojiList.Count)];
    }
}