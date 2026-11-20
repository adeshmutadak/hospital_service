using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;
using Dto.Response;
using Entities.Models.Enum;

namespace Service
{
    public interface IPatientService
    {
        Task<GeneralResponse<AddPatientResponseDto>> AddPatient(AddPatientRequestDto request, long actorUserId, UserRole actorRole);
        Task<string?> GetPatientPhotoFullPathAsync(long patientId);
        Task<GeneralResponse<PagedResponseDto<PatientListItemDto>>> GetPatientList(PatientListQueryDto query);
        Task<GeneralResponse<PatientDetailDto>> GetPatientDetail(long patientId);
    }
}
