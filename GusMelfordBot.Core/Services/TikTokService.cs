namespace GusMelfordBot.Core.Services
{
    using System.IO;
    using System.Threading;
    using Settings;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json.Linq;
    using Telegram.API.TelegramRequests.SendVideo;
    using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;
    using Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard;
    using VideoFile = Telegram.API.TelegramRequests.SendVideo.VideoFile;
    using Requests;
    using System.Threading.Tasks;
    using Telegram.API.TelegramRequests.DeleteMessage;
    using Telegram.API.TelegramRequests.SendMessage;
    using Telegram.Dto;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using GusMelfordBot.Database.Interfaces;
    using Interfaces;
    using System.Linq;
    using Telegram.Dto.UpdateModule;
    
    public class TikTokService : ITikTokService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private readonly IRequestService _requestService;
        private readonly IPlayerService _playerService;
        private readonly CommonSettings _commonSettings;
        private string _currentVideoInfoMessageId;

        private readonly List<string> _emojiList = new()
        {
            "😳", "🥵", "😂", "😘", "❤️", "😜", "💋", "😒", "😍", "🙊", "😆", "😋", "😍", "🙈", "😖", "🥸", "😎", "🥺", "🥳"
        }; 
        
        public TikTokService(
            IDatabaseManager manager,
            IRequestService requestService,
            IGusMelfordBotService gusMelfordBotService,
            IPlayerService playerService,
            CommonSettings commonSettings)
        {
            _databaseManager = manager;
            _gusMelfordBotService = gusMelfordBotService;
            _gusMelfordBotService.OnMessageUpdate += ProcessMessage;
            _gusMelfordBotService.OnCallbackQueryUpdate += ProcessCallbackQuery;
            _requestService = requestService;
            _playerService = playerService;
            _commonSettings = commonSettings;
            //Task.Run(SendStatisticsTimer);
        }

        private void SendStatisticsTimer()
        {
            TimeSpan timeForStatistics = new TimeSpan(21, 0, 0);
            while (true)
            {
                if (DateTime.Now.TimeOfDay.Hours == timeForStatistics.Hours)
                {
                    GetStatistics();
                    Thread.Sleep(600000);
                }
                else
                {
                    Thread.Sleep(60000);
                }
            }
        }
        
        private string GetStatistics()
        {
            List<DAL.User> users = _databaseManager.Context.Set<DAL.User>().ToList();
            List<DAL.TikTok.Video> videos = _databaseManager.Context
                .Set<DAL.TikTok.Video>()
                .Where(x => x.CreatedOn.Date == DateTime.Now.Date)
                .ToList();

            Dictionary<string, int> userAndCount = new Dictionary<string, int>(
                users.ToDictionary(x => x.FirstName, y => videos
                        .Count(x => x.User.FirstName == y.FirstName))
                        .OrderByDescending(x => x.Value));

            string[] medals = {"🥇", "🥈", "🥉"}; //TODO может быть вылет за массив, если будет больше трех пользователей
            int count = 0;
            return $"🥳 Statistics for {DateTime.Now:d}\n\n" +
                   $"{string.Join("\n", userAndCount.Keys.Select(x => medals[count++] + " " + x + userAndCount[x]))}";
        }
        
        private void ProcessCallbackQuery(CallbackQuery callbackQuery)
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
        
        public void ProcessMessage(Message message)
        {
            try
            {
                SetSignature(message);
            
                if (!VerifyTikTokMessage(message))
                {
                    return;
                }

                SaveNewUser(message.From);
                SaveLink(message);
            }
            catch (Exception e)
            {
                SendMessage(message.Chat, $"Something went wrong!\n{e.Message}");
            }
        }

        private void SetSignature(Message message)
        {
            string text = message.Text;
            if (string.IsNullOrEmpty(text) || !text.Contains(Constants.SetCommand))
            {
                return;
            }
            
            string signature = text.Replace(Constants.SetCommand, "");
            DAL.TikTok.Video video = _databaseManager.Context.Set<DAL.TikTok.Video>()
                .OrderBy(x=>x.CreatedOn)
                .Include(x => x.User)
                .LastOrDefault(x => x.User.TelegramUserId == message.From.Id);
                
            if (video is null)
            {
                return;
            }
                
            video.Signature = signature;
            _databaseManager.Context.SaveChanges();
        }
        
        private void SaveNewUser(User user)
        {
            if (_databaseManager.Context.Set<DAL.User>().FirstOrDefault(x => x.TelegramUserId == user.Id) is not null)
            {
                return;
            }

            _databaseManager.Context.Add(
                new DAL.User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.Username,
                    TelegramUserId = user.Id
                });

            _databaseManager.Context.SaveChanges();
        }

        private void SaveLink(Message message)
        {
            var video = CreateVideo(message.Text);
            if (video is null)
            {
                return;
            }
            
            DAL.TikTok.Video oldVideo = _databaseManager.Context.Set<DAL.TikTok.Video>()
                .FirstOrDefaultAsync(v => v.RefererLink == video.RefererLink).Result;
            if (oldVideo is not null)
            {
                DeleteMessage(message);
                return;
            }
            
            _databaseManager.Context.Add(video);
            _databaseManager.Context.SaveChanges();
            
            int counter = _databaseManager.Context.Set<DAL.TikTok.Video>().Count();
            SendMessage(message.Chat, 
                $"{_emojiList[new Random().Next(0, _emojiList.Count)]} " +
                $"{video.User?.FirstName} sent {counter}\n{video.RefererLink}");
            DeleteMessage(message);
        }

        public DAL.TikTok.Video CreateVideo(string link, long userId = 443763853)
        {
            string videoLink = link.Split(new []{' ', '\n'}).FirstOrDefault(l =>
                l.Contains(Constants.TikTokVMDomain) 
                || l.Contains(Constants.TikTokMDomain)
                || l.Contains(Constants.TikTokWWWDomain))?.Trim();

            if (string.IsNullOrEmpty(videoLink))
            {
                return null;
            }
            
            string signature = link.Replace(videoLink, "");
            
            HttpRequestMessage requestMessage =
                new Request(videoLink)
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();

            HttpResponseMessage httpResponseMessage = _requestService.ExecuteAsync(requestMessage).Result;
            Uri uri = httpResponseMessage.RequestMessage?.RequestUri;
            
            requestMessage =
                new Request(uri?.ToString())
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();
            
            httpResponseMessage = _requestService.ExecuteAsync(requestMessage).Result;
            uri = httpResponseMessage.RequestMessage?.RequestUri;
            
            string referer = string.Empty;
            if (uri is not null)
            {
                referer = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
            }

            var user = _databaseManager.Context.Set<DAL.User>()
                .FirstOrDefault(u => u.TelegramUserId == userId);
            
            return new DAL.TikTok.Video {
                User = user,
                SentLink = videoLink,
                RefererLink = referer,
                Signature = signature
            };
        }
        
        private void SendVideo(CallbackQuery callbackQuery, string[] data)
        {
            DAL.TikTok.Video video = _databaseManager.Context.Set<DAL.TikTok.Video>()
                .FirstOrDefaultAsync(v=>string.Equals(v.Id.ToString(), data[1])).Result;

            try
            {
                if (video != null)
                {
                    _gusMelfordBotService.SendVideoAsync(new SendVideoParameters
                    {
                        ChatId = callbackQuery.FromUser.Id,
                        Video = new VideoFile(new MemoryStream(_playerService.CurrentVideoFile.VideoArray), "video")
                    });
                }
            }
            catch (Exception e)
            {
                //ingnore
            }
        }

        private void SendMessage(Chat chat, string message)
        {
            _gusMelfordBotService.SendMessage(
                new SendMessageParameters
                {
                    Text = message,
                    ChatId = chat.Id,
                    DisableNotification = true,
                    DisableWebPagePreview = true
                });
        }
        
        private void DeleteMessage(Message message)
        {
            _gusMelfordBotService.DeleteMessage(
                new DeleteMessageParameters
                {
                    ChatId = message.Chat.Id,
                    MessageId = message.MessageId
                });
        }

        private static bool VerifyTikTokMessage(Message message)
        {
            string text = message.Text;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            
            return Constants.TikTokDomains.FirstOrDefault(x => text.Contains(x)) is not null;
        }

        public async Task SendVideoInfo()
        {
            DAL.TikTok.Video video = _playerService.CurrentVideo;
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup();
            KeyboardRaw<InlineKeyboardButton> keyboardRaw = new KeyboardRaw<InlineKeyboardButton>();
            
            keyboardRaw.AddButtons(new List<InlineKeyboardButton>
            {
                new () {
                    Text = TikTokCallbackQueryButton.Like,
                    CallbackData = TikTokCallbackQueryButton.Like + " " + video.Id
                },
                new () {
                    Text = TikTokCallbackQueryButton.Save,
                    CallbackData = TikTokCallbackQueryButton.Save + " " + video.Id
                }
            });
            
            inlineKeyboardMarkup.AddRaw(keyboardRaw);
            HttpResponseMessage httpResponseMessage = await _gusMelfordBotService.SendMessage(
                new SendMessageParameters
                {
                    ChatId = _commonSettings.TikTokSettings.TikTokChatId,
                    Text = "GusMelfordBot Player 🥵🥵🥵\n\n" +
                           $"{video.Id}\n" +
                           $"{video.User.FirstName}\n" +
                           $"{video.RefererLink}\n\n" +
                           $"{GetStatistics()}",
                    ReplyMarkup = inlineKeyboardMarkup
                });

            _currentVideoInfoMessageId =
                JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync())["result"]?["message_id"]
                    ?.ToString();
        }

        public async Task DeleteVideoInfo()
        {
            if (string.IsNullOrEmpty(_currentVideoInfoMessageId))
            {
                return;
            }
            
            await _gusMelfordBotService.DeleteMessage(
                new DeleteMessageParameters
                {
                    ChatId = _commonSettings.TikTokSettings.TikTokChatId,
                    MessageId = int.Parse(_currentVideoInfoMessageId)
                });
        }
    }
    
    public static class TikTokCallbackQueryButton
    {
        public const string Like = "❤️";
        public const string Save = "💾";
    }
}