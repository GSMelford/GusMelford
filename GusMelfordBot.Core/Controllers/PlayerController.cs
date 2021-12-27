namespace GusMelfordBot.Core.Controllers
{
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    
    [ApiController]
    [Route("player")]
    public class PlayerController : Controller
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IPlayerService _playerService;
        private readonly ITikTokService _tikTokService;
        
        private const string ContentType = "video/mp4";
        
        public PlayerController(
            ILogger<PlayerController> logger,
            IPlayerService playerService,
            ITikTokService tikTokService)
        {
            _logger = logger;
            _playerService = playerService;
            _tikTokService = tikTokService;
        }

        [HttpGet("start")]
        public async Task<JsonResult> Start()
        { 
            await _playerService.Start();
            return Json(new {Start = "Ok"});
        }
        
        [HttpGet("video/new/next")]
        public async Task<JsonResult> GetNextVideoStream()
        {
            return Json(await _playerService.SetNextVideo());
        }
        
        [HttpGet("video/new/prev")]
        public async Task<JsonResult> GetPreviousVideoStream()
        {
            return Json(await _playerService.SetPreviousVideo());
        }
        
        [HttpGet("video/current")]
        public async Task<FileResult> GetCurrentVideo()
        {
            await _tikTokService.DeleteVideoInfo();
            FileResult fileStreamResult =
                new FileContentResult(_playerService.CurrentVideoFile.VideoArray, ContentType);
            HttpContext.Response.Headers.Add("Content-Length", _playerService.CurrentVideoFile.VideoArray.Length.ToString());
            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
            await _tikTokService.SendVideoInfo();
            return fileStreamResult;
        }
    }
}