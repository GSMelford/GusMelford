using GusMelfordBot.DAL;

namespace GusMelfordBot.Core.Applications.MemesChatApp.Player;

using System;
using System.IO;
using System.Net.Http;
using Settings;
using Newtonsoft.Json.Linq;
using Telegram.API.TelegramRequests.DeleteMessage;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.API.TelegramRequests.SendVideo;
using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;
using Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard;
using Telegram.Dto.UpdateModule;
using GusMelfordBot.Core.Interfaces;
using Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
using GusMelfordBot.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
    
public class PlayerService : IPlayerService
{
    private readonly IDatabaseManager _databaseManager;
    private readonly IRequestService _requestService;
    private readonly IGusMelfordBotService _gusMelfordBotService;
    private readonly CommonSettings _commonSettings;
    private readonly ILogger<PlayerService> _logger;
        
    private List<TikTokVideoContent> _videos;
    private int _cursor;
    private string _playerInformationMessageId;

    private TikTokVideoContent CurrentContent { get; set; }
    public byte[] CurrentContentBytes { get; set; }

    public PlayerService(
        ILogger<PlayerService> logger,
        IDatabaseManager databaseManager,
        IRequestService requestService,
        IGusMelfordBotService gusMelfordBotService,
        CommonSettings commonSettings)
    {
        _databaseManager = databaseManager;
        _requestService = requestService;
        _gusMelfordBotService = gusMelfordBotService;
        _logger = logger;
        _commonSettings = commonSettings;
    }

    public async Task SetRandom(int number)
    {
        _cursor = -1;
        _videos = new List<TikTokVideoContent>();
        List<Guid> guids = await _databaseManager.Context
            .Set<TikTokVideoContent>()
            .Select(x => x.Id)
            .ToListAsync();

        Random random = new Random();
        HashSet<int> useVideos = new HashSet<int>();
            
        for (int i = 0; i < number; i++)
        {
            int videoNumber;
            do
            { 
                videoNumber = random.Next(0, guids.Count);
            } while (!useVideos.Add(videoNumber));
            _videos.Add(await _databaseManager.Context
                .Set<TikTokVideoContent>()
                .Include(x=>x.User)
                .FirstOrDefaultAsync(x=>x.Id == guids[videoNumber]));
        }
    }
        
    public async Task SetNotViewed()
    {
        _cursor = -1;
        _videos = new List<TikTokVideoContent>();
        _videos = await _databaseManager.Context
            .Set<TikTokVideoContent>()
            .Include(video => video.User)
            .Where(x => !x.IsViewed)
            .OrderBy(x => x.CreatedOn)
            .ToListAsync();
    }
        
    public async Task ProcessCallbackQuery(CallbackQuery callbackQuery)
    {
        //TODO Сделать нормально
        string[] data = callbackQuery.Data.Split(" ");
        string button = data[0] + " " + data[1];
            
        if (button.Contains(TikTokCallbackQueryButton.Save))
        {
            await SendVideo(callbackQuery, data);
            return;
        }
            
        if(button.Contains(TikTokCallbackQueryButton.Anime))
        {
            CurrentContent.AccompanyingCommentary =
                $"{CurrentContent.AccompanyingCommentary} #{nameof(TikTokCallbackQueryButton.Anime)}";
        }
        else if(button.Contains(TikTokCallbackQueryButton.Film))
        {
            CurrentContent.AccompanyingCommentary =
                $"{CurrentContent.AccompanyingCommentary} #{nameof(TikTokCallbackQueryButton.Film)}";
        }
        else if(button.Contains(TikTokCallbackQueryButton.Horny))
        {
            CurrentContent.AccompanyingCommentary =
                $"{CurrentContent.AccompanyingCommentary} #{nameof(TikTokCallbackQueryButton.Horny)}";
        }
        else if(button.Contains(TikTokCallbackQueryButton.Wallpaper))
        {
            CurrentContent.AccompanyingCommentary =
                $"{CurrentContent.AccompanyingCommentary} #{nameof(TikTokCallbackQueryButton.Wallpaper)}";
        }
        else if(button.Contains(TikTokCallbackQueryButton.ChatCollection))
        {
            CurrentContent.AccompanyingCommentary =
                $"{CurrentContent.AccompanyingCommentary} #{nameof(TikTokCallbackQueryButton.ChatCollection)}";
        }
            
        _databaseManager.Context.Update(CurrentContent);
        await _databaseManager.Context.SaveChangesAsync();
    }
        
    private async Task SendVideo(CallbackQuery callbackQuery, string[] data)
    {
        TikTokVideoContent video = _databaseManager.Context.Set<TikTokVideoContent>()
            .FirstOrDefaultAsync(v=>string.Equals(v.Id.ToString(), data[2])).Result;

        try
        {
            if (video == null) return;
            if (CurrentContentBytes is not null)
            {
                await _gusMelfordBotService.SendVideoAsync(new SendVideoParameters
                {
                    ChatId = callbackQuery.FromUser.Id,
                    Video = new VideoFile(new MemoryStream(CurrentContentBytes), "video")
                });
            }
        }
        catch (Exception e)
        {
            _logger.LogError("We were unable to send the video. Error {Message}", e.Message);
        }
    }
        
    public async Task<PlayerInfo> SetNextVideo()
    {
        if (_videos is null)
        {
            await SetNotViewed();
        }
            
        do
        {
            if (_cursor + 1 >= _videos.Count)
            {
                _cursor = -1;
            }
                
            if (CurrentContent is not null)
            {
                CurrentContent.IsViewed = true;
            }
                
            CurrentContent = _videos[++_cursor];
            CurrentContentBytes = await GetVideoBytes(CurrentContent);
            if (CurrentContentBytes is null)
            {
                CurrentContent.IsValid = false;
            }
        } while (CurrentContentBytes == null);

        CurrentContent.IsValid = true;
        await _databaseManager.Context.SaveChangesAsync();
        await UpdatePlayerInformation();
        return GetVideoInfoForPlayer();
    }
        
    public async Task<PlayerInfo> SetPreviousVideo()
    {
        if (_videos is null)
        {
            await SetNotViewed();
        }
            
        do
        {
            if (_cursor - 1 < 0)
            {
                _cursor = _videos.Count;
            }
                
            if (CurrentContentBytes is null)
            {
                CurrentContent.IsValid = false;
            }
                
            CurrentContent = _videos[--_cursor];
            CurrentContentBytes = await GetVideoBytes(CurrentContent);
        } while (CurrentContentBytes == null);

        CurrentContent.IsValid = true;
        await _databaseManager.Context.SaveChangesAsync();
        await UpdatePlayerInformation();
        return GetVideoInfoForPlayer();
    }

    private PlayerInfo GetVideoInfoForPlayer()
    {
        return new PlayerInfo
        {
            SenderName = CurrentContent.User.FirstName + " " + CurrentContent.User.LastName,
            AccompanyingCommentary = CurrentContent.AccompanyingCommentary,
            AuthorDescription = CurrentContent.Description,
            SentDateTime = CurrentContent.CreatedOn.ToString("g")
        };
    }
        
    private async Task<byte[]> GetVideoBytes(TikTokVideoContent video)
    {
        VideoDownloader videoDownloader = new VideoDownloader(_requestService, _logger);
        return await videoDownloader.DownloadTikTokVideo(video);
    }

    private async Task UpdatePlayerInformation()
    {
        await DeletePlayerInformation();
        await SendPlayerInformation();
    }
        
    private async Task SendPlayerInformation()
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup();
        KeyboardRaw<InlineKeyboardButton> keyboardRawFirst = new KeyboardRaw<InlineKeyboardButton>();
        KeyboardRaw<InlineKeyboardButton> keyboardRawSecond = new KeyboardRaw<InlineKeyboardButton>();
            
        keyboardRawFirst.AddButtons(new List<InlineKeyboardButton>
        {
            new () {
                Text = TikTokCallbackQueryButton.Anime,
                CallbackData = TikTokCallbackQueryButton.Anime + " " + CurrentContent.Id
            },
            new () {
                Text = TikTokCallbackQueryButton.Horny,
                CallbackData = TikTokCallbackQueryButton.Horny + " " + CurrentContent.Id
            },
            new () {
                Text = TikTokCallbackQueryButton.Film,
                CallbackData = TikTokCallbackQueryButton.Film + " " + CurrentContent.Id
            }
        });
            
        keyboardRawSecond.AddButtons(new List<InlineKeyboardButton>
        {
            new () {
                Text = TikTokCallbackQueryButton.Wallpaper,
                CallbackData = TikTokCallbackQueryButton.Wallpaper + " " + CurrentContent.Id
            },
            new () {
                Text = TikTokCallbackQueryButton.Save,
                CallbackData = TikTokCallbackQueryButton.Save + " " + CurrentContent.Id
            },
            new () {
                Text = TikTokCallbackQueryButton.ChatCollection,
                CallbackData = TikTokCallbackQueryButton.ChatCollection + " " + CurrentContent.Id
            }
        });
            
        inlineKeyboardMarkup.AddRaw(keyboardRawFirst);
        inlineKeyboardMarkup.AddRaw(keyboardRawSecond);
            
        HttpResponseMessage httpResponseMessage = await _gusMelfordBotService.SendMessageAsync(
            new SendMessageParameters
            {
                ChatId = -1001529315725, //TODO Сделать нормально
                Text = $"GusMelfordBot Player v. {_commonSettings.PlayerVersion} 🥵🥵🥵\n\n" +
                       $"{CurrentContent.Id}\n" +
                       $"{CurrentContent.User.FirstName} {CurrentContent.User.LastName}\n" +
                       $"{CurrentContent.RefererLink}\n\n",
                ReplyMarkup = inlineKeyboardMarkup
            });

        _playerInformationMessageId =
            JToken.Parse(httpResponseMessage.Content.ReadAsStringAsync().Result)["result"]?["message_id"]
                ?.ToString();
    }
        
    private async Task DeletePlayerInformation()
    {
        if (string.IsNullOrEmpty(_playerInformationMessageId))
        {
            return;
        }
            
        await _gusMelfordBotService.DeleteMessageAsync(
            new DeleteMessageParameters
            {
                ChatId = -1001529315725,
                MessageId = int.Parse(_playerInformationMessageId)
            });
    }
}
    
public static class TikTokCallbackQueryButton
{
    public const string Anime = "Anime ❤️";
    public const string Wallpaper = "Wallpaper 🎇";
    public const string Film = "Film 🎬";
    public const string Horny = "Horny 🥵";
    public const string ChatCollection = "All 💾";
    public const string Save = "Yourself 💾";
}