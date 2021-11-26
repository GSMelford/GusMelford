namespace GusMelfordBot.Core.Controllers
{
    using System.Threading.Tasks;
    using GusMelfordBot.Database.Interfaces;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    
    [ApiController]
    public class GusMelfordBotController : Controller
    {
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private readonly IDataService _dataService;
        
        public GusMelfordBotController(
            ILogger<GusMelfordBotController> logger, 
            IGusMelfordBotService gusMelfordBotService,
            IDatabaseManager databaseManager,
            ITikTokService tikTokService,
            IDataService dataService)
        {
            _dataService = dataService;
            _gusMelfordBotService = gusMelfordBotService;
        }

        [HttpGet("start")]
        public ActionResult<string> Start()
        {
            _gusMelfordBotService.StartListenUpdate();
            return new ActionResult<string>("GusMelfordBot worked...");
        }

        [HttpGet("video/new")]
        public async Task<JsonResult> GetUnwatchVideo()
        {
            return Json(await _dataService.GetUnwatchTikTokVideo());
        }
        
        [HttpGet("video")]
        public async Task<JsonResult> GetUnwatchVideo(
            [FromQuery] string takeDateSince, 
            [FromQuery] string takeDateUntil)
        {
            return Json(await _dataService.GetTikTokVideo(takeDateSince, takeDateUntil));
        }
    }
}