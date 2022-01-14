using System.Linq;
using GusMelfordBot.Core.Interfaces;

namespace GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using Telegram.API.TelegramRequests.DeleteMessage;
    using Telegram.API.TelegramRequests.SendMessage;
    using Telegram.Dto;
    using Telegram.Dto.UpdateModule;

    public class TikTokService : ITikTokService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private readonly IRequestService _requestService;
        private readonly ILogger<TikTokService> _logger;
        
        public TikTokService(
            ILogger<TikTokService> logger,
            IDatabaseManager manager,
            IRequestService requestService,
            IGusMelfordBotService gusMelfordBotService)
        {
            _databaseManager = manager;
            _gusMelfordBotService = gusMelfordBotService;
            _requestService = requestService;
            _logger = logger;
        }

        public void ProcessMessage(Message message)
        {
            try
            {
                SaveUserIfNew(message.From);
                var tokVideoContent = GetContentIfNew(message);

                if (tokVideoContent is not null)
                {
                    string messageId = SendMessageToTelegram(tokVideoContent.User.FirstName, tokVideoContent.RefererLink, message.Chat.Id);
                    tokVideoContent.MessageId = int.Parse(messageId);
                    
                    _databaseManager.Context.Add(tokVideoContent);
                    _databaseManager.Context.SaveChanges();
                }

                DeleteMessageInTelegram(message.Chat.Id, message.MessageId);
            }
            catch (Exception e)
            {
                _logger.LogError("We were unable to save tik tok video content.\n{Message}", e.Message);
            }
        }
        
        private void SaveUserIfNew(User user)
        {
            DAL.User botUser = _databaseManager.Context
                .Set<DAL.User>()
                .FirstOrDefault(x => x.TelegramUserId == user.Id);
            
            if (botUser is not null)
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
        
        private TikTokVideoContent GetContentIfNew(Message message)
        {
            TikTokServiceHelper tikTokServiceHelper = new TikTokServiceHelper();
            string sentLink = tikTokServiceHelper.WithdrawSendLink(message.Text);
            string refererLink = tikTokServiceHelper.WithdrawRefererLink(_requestService, sentLink);
            
            TikTokVideoContent tokVideoContent = 
                tikTokServiceHelper.BuildTikTokVideoContent(_databaseManager, sentLink, refererLink, message.From.Id);
            
            if (tokVideoContent is null)
            {
                return null;
            }
            
            TikTokVideoContent oldTokVideoContent = _databaseManager.Context.Set<TikTokVideoContent>()
                .FirstOrDefault(v => v.RefererLink == tokVideoContent.RefererLink);
            
            return oldTokVideoContent is not null ? null : tokVideoContent;
        }

        private string SendMessageToTelegram(string firstName, string refererLink, long chatId)
        {
            int tikTokVideoContentCount = _databaseManager.Context.Set<TikTokVideoContent>().Count();
            HttpResponseMessage httpResponseMessage = _gusMelfordBotService.SendMessage(new SendMessageParameters
            {
                Text = $"{Helper.GetRandomEmoji()} {firstName} sent meme №{tikTokVideoContentCount + 1}\n{refererLink}",
                ChatId = chatId,
                DisableNotification = true,
                DisableWebPagePreview = true
            });

            return WithdrawMessageId(httpResponseMessage);
        }

        private string WithdrawMessageId(HttpResponseMessage httpResponseMessage)
        {
            JToken token = JToken.Parse(httpResponseMessage.Content.ReadAsStringAsync().Result);
            return token["result"]?["message_id"]?.ToString();
        }
        
        private void DeleteMessageInTelegram(long chatId, int messageId)
        {
            _gusMelfordBotService.DeleteMessage(new DeleteMessageParameters
            {
                ChatId = chatId,
                MessageId = messageId
            });
        }
        
        /*private string GetStatistics()
        {
            List<DAL.User> users = _databaseManager.Context.Set<DAL.User>().ToList();
            List<TikTokVideoContent> videos = _databaseManager.Context
                .Set<TikTokVideoContent>()
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
        
        private void SendVideo(CallbackQuery callbackQuery, string[] data)
        {
            TikTokVideoContent video = _databaseManager.Context.Set<TikTokVideoContent>()
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

        public async Task SendVideoInfo()
        {
            TikTokVideoContent video = _playerService.CurrentVideo;
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
            HttpResponseMessage httpResponseMessage = await _gusMelfordBotService.SendMessageAsync(
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
            
            await _gusMelfordBotService.DeleteMessageAsync(
                new DeleteMessageParameters
                {
                    ChatId = _commonSettings.TikTokSettings.TikTokChatId,
                    MessageId = int.Parse(_currentVideoInfoMessageId)
                });
        }*/
    }
    
    public static class TikTokCallbackQueryButton
    {
        public const string Like = "❤️";
        public const string Save = "💾";
    }
}