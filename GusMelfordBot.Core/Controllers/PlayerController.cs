using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Controllers
{
    using System.IO;
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
        
        private const string CONTENT_TYPE = "video/mp4";
        
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
        public async Task<FileStreamResult> GetCurrentVideo([FromQuery] string updated)
        {
            await _tikTokService.DeleteVideoInfo();
            FileStreamResult fileStreamResult =
                new FileStreamResult(new MemoryStream(_playerService.CurrentVideoFile.VideoArray), CONTENT_TYPE);
            
            HttpContext.Response.Headers.Add("Content-Length", _playerService.CurrentVideoFile.VideoArray.Length.ToString());
            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache");
            HttpContext.Response.Headers.Add("Expires", "0");
           
            await _tikTokService.SendVideoInfo();
            _logger.LogInformation("Update to new video. Request time: {Updated}", updated);
            return fileStreamResult;
        }
        
        [HttpPost("video/new")]
        public void SetNewVideos([FromBody]string body)
        {
            _playerService.AddNewVideos(JToken.Parse(body));
        }
    }
}