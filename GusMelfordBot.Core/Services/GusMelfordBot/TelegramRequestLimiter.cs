using System;
using System.Diagnostics;

namespace GusMelfordBot.Core.Services.GusMelfordBot
{
    public class TelegramRequestLimiter
    {
        private const int MAX_REQUEST = 20;
        private readonly TimeSpan _requestPerTime = TimeSpan.FromMinutes(1);
        private int _requestCounter;
        private Stopwatch _time;
        
        public void CheckAbility()
        {
            if (_time.Elapsed.Seconds >= _requestPerTime.Seconds)
            {
                _requestCounter = 0;
            }
            
            if (_requestCounter == 0)
            {
                _time.Start();
            }

            if (_time.Elapsed.Minutes < _requestPerTime.Minutes && _requestCounter > MAX_REQUEST)
            {
                _requestCounter++;
                return;
            }
            else
            {
                
            }
        }
    }
}