namespace GusMelfordBot.Core.Applications.MemesChatApp.Player
{
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
        private int _cursor = -1;
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
            Init();
        }

        private void Init()
        {
            _videos = _databaseManager.Context
                .Set<TikTokVideoContent>()
                .Include(video => video.User)
                .Where(x => !x.IsViewed)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync().Result;
        }
        
        public void ProcessCallbackQuery(CallbackQuery callbackQuery)
        {
            string[] data = callbackQuery.Data.Split(" ");

            switch (data[0])
            {
                case TikTokCallbackQueryButton.Save:
                {
                    SendVideo(callbackQuery, data);
                    break;
                }
            }
        }
        
        private void SendVideo(CallbackQuery callbackQuery, string[] data)
        {
            TikTokVideoContent video = _databaseManager.Context.Set<TikTokVideoContent>()
                .FirstOrDefaultAsync(v=>string.Equals(v.Id.ToString(), data[1])).Result;

            try
            {
                if (video == null) return;
                if (CurrentContentBytes is not null)
                {
                    _gusMelfordBotService.SendVideo(new SendVideoParameters
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
            UpdatePlayerInformation();
            return GetVideoInfoForPlayer();
        }
        
        public async Task<PlayerInfo> SetPreviousVideo()
        {
            do
            {
                if (_cursor - 1 < 0)
                {
                    _cursor = _videos.Count;
                }

                CurrentContent = _videos[--_cursor];
                CurrentContentBytes = await GetVideoBytes(CurrentContent);
                if (CurrentContentBytes is null)
                {
                    CurrentContent.IsValid = false;
                }
            } while (CurrentContentBytes == null);

            CurrentContent.IsValid = true;
            await _databaseManager.Context.SaveChangesAsync();
            UpdatePlayerInformation();
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

        private void UpdatePlayerInformation()
        {
            DeletePlayerInformation();
            SendPlayerInformation();
        }
        
        private void SendPlayerInformation()
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup();
            KeyboardRaw<InlineKeyboardButton> keyboardRaw = new KeyboardRaw<InlineKeyboardButton>();
            
            keyboardRaw.AddButtons(new List<InlineKeyboardButton>
            {
                new () {
                    Text = TikTokCallbackQueryButton.Save,
                    CallbackData = TikTokCallbackQueryButton.Save + " " + CurrentContent.Id
                }
            });
            
            inlineKeyboardMarkup.AddRaw(keyboardRaw);
            HttpResponseMessage httpResponseMessage = _gusMelfordBotService.SendMessage(
                new SendMessageParameters
                {
                    ChatId = -1001529315725, //TODO Сделать нормально
                    Text = $"GusMelfordBot Player v. {_commonSettings.PlayerVersion} 🥵🥵🥵\n\n" +
                           $"{CurrentContent.Id}\n" +
                           $"{CurrentContent.User.FirstName}\n" +
                           $"{CurrentContent.RefererLink}\n\n",
                    ReplyMarkup = inlineKeyboardMarkup
                });

            _playerInformationMessageId =
                JToken.Parse(httpResponseMessage.Content.ReadAsStringAsync().Result)["result"]?["message_id"]
                    ?.ToString();
        }
        
        private void DeletePlayerInformation()
        {
            if (string.IsNullOrEmpty(_playerInformationMessageId))
            {
                return;
            }
            
            _gusMelfordBotService.DeleteMessage(
                new DeleteMessageParameters
                {
                    ChatId = _commonSettings.TikTokSettings.TikTokChatId,
                    MessageId = int.Parse(_playerInformationMessageId)
                });
        }
    }
    
    public static class TikTokCallbackQueryButton
    {
        public const string Like = "❤️";
        public const string Save = "💾";
    }
}