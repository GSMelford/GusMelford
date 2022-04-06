using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok;

using Telegram.Dto.UpdateModule;
using System.Threading;
using RestSharp;
using System;
using System.Linq;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
using GusMelfordBot.Database.Interfaces;
    
public static class TikTokServiceHelper
{
    private static readonly RestClient Client = new ();
        
    public static string WithdrawSendLink(string messageText)
    {
        return messageText.Split(' ', '\n')
            .FirstOrDefault(x => x.Contains(Constants.TikTok))?.Trim();
    }

    public static TikTokVideoContent BuildTikTokVideoContent(
        IDatabaseManager databaseManager,
        string sentLink,
        string refererLink,
        long userId)
    {
        var user = databaseManager.Context.Set<DAL.User>()
            .FirstOrDefault(u => u.TelegramUserId == userId);

        return new TikTokVideoContent
        {
            User = user,
            SentLink = sentLink,
            RefererLink = refererLink,
            Description = null,
            AccompanyingCommentary = null
        };
    }

    public static string WithdrawRefererLink(string sentLink)
    {
        RestResponse response = Client.ExecuteAsync(new RestRequest(sentLink)).Result;
            
        Uri uri = response.ResponseUri;
        response = Client.ExecuteAsync(new RestRequest(uri?.ToString())).Result;
            
        uri = response.ResponseUri;
        string refererLink = string.Empty;
        if (uri is not null)
        {
            refererLink = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
        }

        Thread.Sleep(1000);
            
        return refererLink;
    }
        
    public static string SetAccompanyingCommentaryIfExist(string userName, string text)
    {
        text = Regex.Replace(text, @"\s+", " ");
        string[] words = text.Trim().Split(" ");

        if (words.Length == 1)
        {
            return string.Empty;
        }
            
        IEnumerable<string> wordsWithoutTikTokLink = words.Where(x => !x.Contains(Constants.TikTok));
        return $"{userName}: {string.Join(" ", wordsWithoutTikTokLink)}";
    }
        
    public static string GetProcessMessage(Message message)
    {
        return $"⚙️{message.From.FirstName} {message.From.LastName} sent\n" +
               $"{message.Text}\n" +
               "This tiktok is being processed";
    }

    public static string GetEditedMessage(TikTokVideoContent tikTokVideoContent, int count, string comment)
    {
        string text = $"{Helper.GetRandomEmoji()} " +
                      $"{tikTokVideoContent.User.FirstName} {tikTokVideoContent.User.LastName} sent meme №{count + 1}\n" +
                      $"{tikTokVideoContent.RefererLink}";

        if (!string.IsNullOrEmpty(comment))
        {
            text += $"\n{comment}";
        }

        return text;
    }

    public static string GetEditedMessageAboutExist(Message message)
    {
        return $"{Helper.GetRandomEmoji()} " +
               $"{message.From.FirstName} {message.From.LastName} sent meme which existed\n" +
               $"{message.Text}";
    }
        
    public static string GetEditedMessageWhetException(Message message)
    {
        return $"😐 {message.From.FirstName} {message.From.LastName} sent meme and got an error\n" +
               $"{message.Text}";
    }
}