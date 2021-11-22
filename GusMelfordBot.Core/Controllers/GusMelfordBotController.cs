namespace GusMelfordBot.Core.Controllers
{
    using GusMelfordBot.Database.Interfaces;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    
    [ApiController]
    [Route("[controller]")]
    public class GusMelfordBotController : Controller
    {
        private readonly ILogger<GusMelfordBotController> _logger;
        private readonly IGusMelfordBotService _gusMelfordBotService;
        
        public GusMelfordBotController(
            ILogger<GusMelfordBotController> logger, 
            IGusMelfordBotService gusMelfordBotService,
            IDatabaseContext databaseContext,
            ITikTokService tikTokService)
        {
            _logger = logger;
            _gusMelfordBotService = gusMelfordBotService;
        }

        [HttpGet("gusmelfordbot/start")]
        public IActionResult Start()
        {
            if (_gusMelfordBotService.GetStatus())
            {
                _logger.LogInformation("GusMelfordBot already started");
                return Ok();
            }
            _gusMelfordBotService.StartListenUpdate();
            _logger.LogInformation("GusMelfordBot started");

            return Ok();
        }
    }
}