using System.Security.Claims;
using CommonLayer.CommonResponse;
using Dto.Request;
using Dto.Response;
using Entities.Models.Enum;
using Hospital_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Hospital_Service.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [Authorize(Roles = "DeptHead,Doctor,Assistant")]
        [HttpPost("add")]
        [RequestSizeLimit(4 * 1024 * 1024)]   // room for a 2 MB photo encoded as base64
        public async Task<ActionResult<GeneralResponse<AddPatientResponseDto>>> AddPatient(AddPatientRequestDto request)
        {
            var actorUserId = long.Parse(User.FindFirst("UserId")!.Value);
            var actorRole = Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)!.Value);

            var response = await _patientService.AddPatient(request, actorUserId, actorRole);
            return StatusCode((int)response.HttpStatusCode, response);
        }

        // Staff can view a patient photo (requires a login token)
        [Authorize(Roles = "Owner,DeptHead,Doctor,Assistant")]
        [HttpGet("{patientId:long}/photo")]
        public async Task<IActionResult> GetPhoto(long patientId)
        {
            var fullPath = await _patientService.GetPatientPhotoFullPathAsync(patientId);
            if (fullPath == null)
                return NotFound();

            var contentType = fullPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg";
            return PhysicalFile(fullPath, contentType);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("list")]
        public async Task<ActionResult<GeneralResponse<PagedResponseDto<PatientListItemDto>>>> GetPatientList([FromQuery] PatientListQueryDto query)
        {
            var response = await _patientService.GetPatientList(query);
            return StatusCode((int)response.HttpStatusCode, response);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("{patientId:long}")]
        public async Task<ActionResult<GeneralResponse<PatientDetailDto>>> GetPatientDetail(long patientId)
        {
            var response = await _patientService.GetPatientDetail(patientId);
            return StatusCode((int)response.HttpStatusCode, response);
        }
    }
}
