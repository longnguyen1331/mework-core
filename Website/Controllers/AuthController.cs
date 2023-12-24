using Contract.Identity.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Services.Auth;

namespace Website.Controllers
{
    [Route("auth/")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignInAsync([FromBody] CreateUserDto input)
        {
            var data = await _authService.SignUpAsync(input);
            if (data == null || data.IsSuccess == false)
                return BadRequest(data?.Message);
            return Ok();
        }

        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignInAsync([FromBody] UserModel input)
        {
            var data = await _authService.AuthAsync(input);
            if (data == null)
                return BadRequest("Thông tin đăng nhập không đúng");
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign-out")]
        public async Task<IActionResult> SignOutAsync()
        {
            await _authService.SignOutAsync(new WebisteUserLogOutModel
            {
                AccessToken = _authService.GetUserToken(),
                DeviceId = _authService.GetDeviceId()
            });

            return Ok();
        }
    }
}