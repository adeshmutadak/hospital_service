using CommonLayer.CommonResponse;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Hospital_Service.Controllers
{

    [Route("v1/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<GeneralResponse<DepartmentResponseDto>>> AddDepartment(AddDepartmentRequestDto request)
        {
            var response = await _departmentService.AddDepartment(request);
            return StatusCode((int)response.HttpStatusCode, response);
        }
        [HttpGet("list")]
        public async Task<ActionResult<GeneralResponse<List<DepartmentListItemDto>>>> GetDepartmentList()
        {
            var response = await _departmentService.GetDepartmentList();
            return StatusCode((int)response.HttpStatusCode, response);
        }
    }
}
