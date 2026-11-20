using CommonLayer.CommonResponse;
using Dto.Request;
using Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Hospital_Service.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<GeneralResponse<AddUserResponseDto>>> AddUser(AddUserRequestDto request)
        {
            var response = await _userService.AddUser(request);
            return StatusCode((int)response.HttpStatusCode, response);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("list")]
        public async Task<ActionResult<GeneralResponse<PagedResponseDto<StaffListItemDto>>>> GetStaffList([FromQuery] StaffListQueryDto query)
        {
            var response = await _userService.GetStaffList(query);
            return StatusCode((int)response.HttpStatusCode, response);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("detail")]
        public async Task<ActionResult<GeneralResponse<StaffDetailDto>>> GetStaffDetail([FromQuery] StaffDetailQueryDto query)
        {
            var response = await _userService.GetStaffDetail(query);
            return StatusCode((int)response.HttpStatusCode, response);
        }
    }
}