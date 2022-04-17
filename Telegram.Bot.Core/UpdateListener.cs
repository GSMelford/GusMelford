using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bot.Api.BotRequests.Interfaces;
using Telegram.API.TelegramRequests.GetUpdates;
using Telegram.Dto.UpdateModule;

namespace Telegram.Bot.Client
{
    public class UpdateListener
    {
        private CancellationTokenSource _updateCancellationTokenSource = new ();
        private const int LIMIT = 100;
        private const int TIMEOUT = 120;
        private int _updateId;
        
        public delegate void MessageHandler(Message message);
        public event MessageHandler OnMessageUpdate;      
        
        public delegate void CallbackQueryHandler(CallbackQuery callbackQuery);
        public event CallbackQueryHandler OnCallbackQueryUpdate;
        
        public delegate void EditedMessageHandler(EditedMessage editedMessage);
        public event EditedMessageHandler OnEditedMessageUpdate;
        
        public async Task StartListenUpdateAsync(
            Func<IParameters, Task<List<Update>>> getUpdates, 
            CancellationToken cancellationToken = default)
        {
            _updateCancellationTokenSource = new CancellationTokenSource();
            cancellationToken.Register(() => _updateCancellationTokenSource.Cancel());
            
            while (!cancellationToken.IsCancellationRequested)
            {
                List<Update> updates = await getUpdates(new GetUpdatesParameters
                {
                    Timeout = TIMEOUT,
                    Limit = LIMIT,
                    Offset = _updateId
                });

                if (updates.Count <= 0)
                {
                    continue;
                }
                
                foreach (Update update in updates)
                {
                    UpdateController(update);
                }

                _updateId = updates[^1].UpdateId + 1;
            }
        }

        private void UpdateController(Update update)
        {
            if (update.Message is not null)
            {
                OnMessageUpdate?.Invoke(update.Message);
            }
            else if(update.CallbackQuery is not null)
            {
                OnCallbackQueryUpdate?.Invoke(update.CallbackQuery);
            }
            else if(update.EditedMessage is not null)
            {
                OnEditedMessageUpdate?.Invoke(update.EditedMessage);
            }
        }
        
        public void StopListenUpdate()
        {
            _updateCancellationTokenSource.Cancel();
        }
    }
}