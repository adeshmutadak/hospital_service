using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto.Request;
using Hospital_Service.Models;

namespace Repository
{
    public interface IPatientRepo
    {
        Task<Doctor?> GetActiveDoctorAsync(long doctorId);
        Task<Doctor?> GetDoctorByUserIdAsync(long userId);
        Task<bool> IsAssistantOfDoctorAsync(long assistantUserId, long doctorId);
        Task AddPatientAsync(Patient patient, long doctorId, long actorUserId, string codePrefix);
        Task<string?> GetPatientPhotoPathAsync(long patientId);

        Task<long?> GetDoctorIdByUserIdAsync(long userId);
        Task<(List<PatientListItemDto> Items, int Total)> GetPatientListAsync(PatientListQueryDto query, long? doctorId);
        Task<PatientDetailDto?> GetPatientDetailAsync(long patientId);
    }
}
