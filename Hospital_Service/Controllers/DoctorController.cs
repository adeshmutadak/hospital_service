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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<GeneralResponse<AddDoctorResponseDto>>> AddDoctor(AddDoctorRequestDto request)
        {
            var response = await _doctorService.AddDoctor(request);
            return StatusCode((int)response.HttpStatusCode, response);
        }
    }
}
