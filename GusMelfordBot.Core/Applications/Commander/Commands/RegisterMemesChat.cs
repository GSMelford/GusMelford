using System.Linq;

namespace GusMelfordBot.Core.Applications.Commander.Commands
{
    using Interfaces;
    using GusMelfordBot.DAL.Applications.MemesChat;
    using GusMelfordBot.Database.Interfaces;
    using Telegram.API.TelegramRequests.SendMessage;
    using Telegram.Dto;
    
    public static class RegisterMemesChat
    {
        public static void Register(
            IDatabaseManager databaseManager, 
            IGusMelfordBotService gusMelfordBotService,
            Chat chat)
        {
            MemesChat memesChat = databaseManager.Context.Set<MemesChat>().FirstOrDefault(x => x.ChatId == chat.Id);

            if (memesChat is not null)
            {
                gusMelfordBotService.SendMessage(new SendMessageParameters
                {
                    Text = "Your chat is already like a chat for memes! And it can't be deleted) 🤪",
                    ChatId = chat.Id
                });
                return;
            }
            
            memesChat = new MemesChat
            {
                ChatId = chat.Id,
                //Type = chat.Type
            };
            
            databaseManager.Context.Add(memesChat);
            databaseManager.Context.SaveChanges();
            gusMelfordBotService.SendMessage(new SendMessageParameters
            {
                Text = "Congratulations! 🥳 Your Chat has become a meme chat! " +
                       "Now you can share different content with your friends " +
                       "(if you have them) and watch them together in the " +
                       "player! 🥵🥵🥵",
                ChatId = chat.Id
            });
        }
    }
}