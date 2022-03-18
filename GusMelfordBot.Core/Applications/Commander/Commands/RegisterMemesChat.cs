namespace GusMelfordBot.Core.Applications.Commander.Commands
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Interfaces;
    using GusMelfordBot.DAL.Applications.MemesChat;
    using GusMelfordBot.Database.Interfaces;
    using Telegram.API.TelegramRequests.SendMessage;
    using Telegram.Dto;
    
    public static class RegisterMemesChat
    {
        public static async Task Register(
            IDatabaseManager databaseManager, 
            IGusMelfordBotService gusMelfordBotService,
            Chat chat)
        {
            //TODO Add save name of chat
            MemesChat memesChat = await databaseManager.Context.Set<MemesChat>()
                .FirstOrDefaultAsync(x => x.ChatId == chat.Id);

            if (memesChat is not null)
            {
                await gusMelfordBotService.SendMessageAsync(new SendMessageParameters
                {
                    Text = "Your chat is already like a chat for memes! And it can't be deleted) 🤪",
                    ChatId = chat.Id
                });
                return;
            }
            
            memesChat = new MemesChat
            {
                ChatId = chat.Id,
                //TODO Add to Bot.Api Type = chat.Type
            };
            
            await databaseManager.Context.AddRangeAsync(memesChat);
            await databaseManager.Context.SaveChangesAsync();
            await gusMelfordBotService.SendMessageAsync(new SendMessageParameters
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