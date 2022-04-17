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
        var updateEntity = JsonConvert.DeserializeObject<global::Telegram.Dto.UpdateModule.Update>(json);
        _logger.LogInformation("Update: {Text}", updateEntity?.Message?.Text);

        try
        {
            if (updateEntity?.Message is not null)
            {
                await _applicationService.ProcessMessage(updateEntity.Message);
            }
        }
        catch
        {
            return false;
        }
        
        return true;
    }
}