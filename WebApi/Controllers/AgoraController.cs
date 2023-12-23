using Application.AppConfigs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/agoras/")]
    public class AgoraController : ControllerBase
    {
        private readonly IAgoraService _agoraService;
        public AgoraController(IAgoraService agoraService)
        {
            _agoraService = agoraService;
        }

        [HttpPost("get-rtc-token")]
        public async Task<IActionResult> GetRtcToken(AgoraRequest setting)
        {
            var result = await _agoraService.CreateRtcToken(setting);
            return Ok(result);
        }
    }
}