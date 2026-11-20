using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using CommonLayer.SecurityHelper;
using Dto.Request;
using Entities.Models.Enum;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;

namespace Service
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepo _doctorRepo;

        public DoctorService(IDoctorRepo doctorRepo)
        {
            _doctorRepo = doctorRepo;
        }

        public async Task<GeneralResponse<AddDoctorResponseDto>> AddDoctor(AddDoctorRequestDto request)
        {
            var code = request.Code.Trim().ToUpperInvariant();

            // 1. User must exist and have the Doctor or DeptHead role
            var user = await _doctorRepo.GetActiveUserAsync(request.UserId);
            if (user == null)
            {
                return Fail("User not found.", HttpStatusCode.NotFound);
            }

            if (user.Role != UserRole.Doctor && user.Role != UserRole.DeptHead)
            {
                return Fail("Only users with the Doctor or Department head role can get a doctor profile.", HttpStatusCode.BadRequest);
            }

            // 2. One doctor profile per user
            if (await _doctorRepo.DoctorProfileExistsAsync(user.Id))
            {
                return Fail("This user already has a doctor profile.", HttpStatusCode.Conflict);
            }

            // 3. Doctor code must be unique
            var codeOwner = await _doctorRepo.GetDoctorByCodeAsync(code);
            if (codeOwner != null)
            {
                return Fail($"Code {code} is already used by {codeOwner.User.FullName}. Try adding initials, e.g. P{code}.", HttpStatusCode.Conflict);
            }

            // 4. Department must be active and have its own code
            var department = await _doctorRepo.GetActiveDepartmentAsync(request.DepartmentId);
            if (department == null)
            {
                return Fail("Department not found.", HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(department.Code))
            {
                return Fail($"{department.Name} has no code yet. Set the department code first.", HttpStatusCode.BadRequest);
            }

            // 5. A DeptHead user becomes the department head, if the department has none
            var makeHead = user.Role == UserRole.DeptHead;
            if (makeHead && department.HeadDoctorId != null)
            {
                return Fail($"{department.Name} already has a head of department.", HttpStatusCode.Conflict);
            }

            // 6. Senior doctor (optional). 0 or null means "reports to no one"
            var reportsToId = request.ReportsToDoctorId is > 0 ? request.ReportsToDoctorId : null;
            if (reportsToId.HasValue)
            {
                var senior = await _doctorRepo.GetActiveDoctorAsync(reportsToId.Value);
                if (senior == null)
                {
                    return Fail($"Senior doctor with ID {reportsToId} was not found. Use the doctor ID, not the user ID.", HttpStatusCode.BadRequest);
                }

                if (senior.DepartmentId != department.Id)
                {
                    return Fail("The senior doctor must be in the same department.", HttpStatusCode.BadRequest);
                }
            }

            var doctor = new Doctor
            {
                UserId = user.Id,
                DepartmentId = department.Id,
                Code = code,
                ReportsToDoctorId = reportsToId,
                Qualification = request.Qualification?.Trim(),
                Specialization = request.Specialization?.Trim(),
                RegistrationNo = request.RegistrationNo?.Trim(),
                ConsultationFee = request.ConsultationFee,
                SlotMinutes = request.SlotMinutes
            };

            try
            {
                await _doctorRepo.AddDoctorAsync(doctor, department, makeHead);
            }
            catch (DbUpdateException)
            {
                return Fail("This user already has a doctor profile, or the code is already used.", HttpStatusCode.Conflict);
            }

            return new GeneralResponse<AddDoctorResponseDto>
            {
                Success = true,
                Message = makeHead
                    ? $"Doctor added and set as head of {department.Name}."
                    : "Doctor added.",
                HttpStatusCode = HttpStatusCode.Created,
                Data = new AddDoctorResponseDto
                {
                    DoctorId = doctor.Id,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mobile = user.Mobile,
                    Code = code,
                    DepartmentId = department.Id,
                    DepartmentName = department.Name,
                    DepartmentCode = department.Code,
                    RoleName = user.Role.ToString(),
                    IsDepartmentHead = makeHead
                }
            };
        }

        private static GeneralResponse<AddDoctorResponseDto> Fail(string message, HttpStatusCode status)
        {
            return new GeneralResponse<AddDoctorResponseDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = status,
                Data = null
            };
        }
    }
}
