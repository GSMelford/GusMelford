namespace GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok
{
    using System.Linq;
    using GusMelfordBot.Core.Interfaces;
    using System;
    using System.Net.Http;
    using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
    using GusMelfordBot.Database.Interfaces;
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
        private readonly ILogger<TikTokService> _logger;
        
        public TikTokService(
            ILogger<TikTokService> logger,
            IDatabaseManager manager,
            IGusMelfordBotService gusMelfordBotService)
        {
            _databaseManager = manager;
            _gusMelfordBotService = gusMelfordBotService;
            _logger = logger;
        }

        public void ProcessMessage(Message message)
        {
            string messageId = SendMessageToTelegram(
                $"⚙️ Processing a new tiktok content\nfrom {message.From.FirstName} ...", message.Chat.Id);
            try
            {
                SaveUserIfNew(message.From);
                var tokVideoContent = GetContentIfNew(message);

                if (tokVideoContent is not null)
                {
                    int tikTokVideoContentCount = _databaseManager.Context.Set<TikTokVideoContent>().Count();
                    
                    _gusMelfordBotService.EditTelegramMessage(
                        message.Chat.Id.ToString(),
                        $"{Helper.GetRandomEmoji()} {message.From.FirstName} " +
                        $"sent meme №{tikTokVideoContentCount + 1}\n{tokVideoContent.RefererLink}",
                        messageId);
                   
                    tokVideoContent.MessageId = int.Parse(messageId);
                    _databaseManager.Context.Add(tokVideoContent);
                    _databaseManager.Context.SaveChanges();
                }
                else
                {
                    _gusMelfordBotService.EditTelegramMessage(
                        message.Chat.Id.ToString(),
                        $"{message.From.FirstName + message.From.LastName} sent a meme that was already in the database",
                        messageId);
                }

                DeleteMessageInTelegram(message.Chat.Id, message.MessageId);
            }
            catch (Exception e)
            {
                _logger.LogError("We were unable to save tik tok video content.\n{Message}", e.Message);

                if (string.IsNullOrEmpty(messageId))
                {
                    return;
                }
                
                _gusMelfordBotService.EditTelegramMessage(message.Chat.Id.ToString(), 
                    $"Bad message from {message.From.FirstName}\n {message.Text}", messageId);
            }
        }
        
        private void SaveUserIfNew(User user)
        {
            DAL.User botUser = _databaseManager.Context
                .Set<DAL.User>()
                .FirstOrDefault(x => x.TelegramUserId == user.Id);
            
            if (botUser is not null)
            {
                if (botUser.FirstName == user.FirstName 
                    && botUser.LastName == user.LastName 
                    && botUser.UserName == user.Username)
                {
                    return;
                }
                
                botUser.FirstName = user.FirstName;
                botUser.LastName = user.LastName;
                botUser.UserName = user.Username;
                _databaseManager.Context.Update(botUser);
                _databaseManager.Context.SaveChanges();
                
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
            string refererLink = tikTokServiceHelper.WithdrawRefererLink(sentLink);
            
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

        private string SendMessageToTelegram(string text, long chatId)
        {
            HttpResponseMessage httpResponseMessage = _gusMelfordBotService.SendMessage(new SendMessageParameters
            {
                Text = text,
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
    }
}