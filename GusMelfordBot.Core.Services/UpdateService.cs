using GusMelfordBot.Core.Domain.Apps;
using GusMelfordBot.Core.Domain.Update;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GusMelfordBot.Core.Services;

public class UpdateService : IUpdateService
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(
        IApplicationService applicationService,
        ILogger<UpdateService> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    public async Task<bool> ProcessUpdate(string json)
    {
        var updateEntity = JsonConvert.DeserializeObject<Telegram.Dto.UpdateModule.Update>(json);
        _logger.LogInformation("Update text: {Text}", updateEntity?.Message?.Text);

        try
        {
            if (updateEntity?.Message is not null)
            {
                await _applicationService.ProcessMessage(updateEntity.Message);
            }
            else if (updateEntity?.CallbackQuery is not null)
            {
                _applicationService.ProcessCallbackQuery(updateEntity.CallbackQuery);
            }
        }
        catch
        {
            return false;
        }
        
        return true;
    }
}