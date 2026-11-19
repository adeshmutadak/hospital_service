using CommonLayer.CommonResponse;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Hospital_Service.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<GeneralResponse<LoginResponseDto>>> Login(LoginRequestDto request)
        {
            var response = await _authService.LoginUser(request);
            return StatusCode((int)response.HttpStatusCode, response);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<BaseResponse>> ChangePassword(ChangePasswordRequestDto request)
        {
            var userId = long.Parse(User.FindFirst("UserId")!.Value);
            var response = await _authService.ChangePassword(userId, request);
            return StatusCode((int)response.HttpStatusCode, response);
        }
    }
}
