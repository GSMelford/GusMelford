using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Domain.Update;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.API.TelegramRequests.GetUpdates;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Services.Modules;

public class LongPoolingUpdateService : IHostedService, IDisposable
{
    private readonly ILogger<LongPoolingUpdateService> _logger;
    private readonly IUpdateService _updateService;
    private readonly IGusMelfordBotService _gusMelfordBotService;
    private const int LIMIT = 100;
    private const int TIMEOUT = 3000;
    private int _updateId;
    private Timer _timer = null!;
    private bool _isRun;
    
    public LongPoolingUpdateService(
        ILogger<LongPoolingUpdateService> logger,
        IGusMelfordBotService gusMelfordBotService, 
        IUpdateService updateService)
    {
        _logger = logger;
        _gusMelfordBotService = gusMelfordBotService;
        _updateService = updateService;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LongPoolingUpdateService started");
        
        _timer = new Timer(CheckUpdates, _isRun, TimeSpan.Zero, TimeSpan.FromSeconds(25));
        
        return Task.CompletedTask;
    }

    private void CheckUpdates(object? state)
    {
        if ((bool)state!)
        {
            return;
        }
        
        try
        {
            _isRun = true;
            List<Update> updates = _gusMelfordBotService.GetUpdates(new GetUpdatesParameters
            {
                Limit = LIMIT,
                Offset = _updateId,
                Timeout = TIMEOUT
            }).Result;

            if (updates.Count <= 0)
            {
                return;
            }

            foreach (string json in updates.Select(JsonConvert.SerializeObject))
            {
                bool result = _updateService.ProcessUpdate(json).Result;
                if (!result)
                {
                    _logger.LogWarning("Content not saved. Error in ProcessUpdate {Json}", json);
                }
            }

            _updateId = updates[^1].UpdateId + 1;
            _isRun = false;
        }
        catch (global::System.Exception e)
        {
            _logger.LogCritical("Error processing update! Error message: {Error}", e.Message);
        }
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LongPoolingUpdateService Hosted Service is stopping");

        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}