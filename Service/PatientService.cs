using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using CommonLayer.PhotoUpload;
using CommonLayer.SecurityHelper;
using Dto.Request;
using Dto.Response;
using Entities.Models.Enum;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;

namespace Service
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepo _patientRepo;
        private readonly ISecurityHelper _securityHelper;
        private readonly IConfiguration _config;
        private readonly IFileService _fileService;

        public PatientService(IPatientRepo patientRepo, ISecurityHelper securityHelper,
                              IConfiguration config, IFileService fileService)
        {
            _patientRepo = patientRepo;
            _securityHelper = securityHelper;
            _config = config;
            _fileService = fileService;
        }

        public async Task<GeneralResponse<AddPatientResponseDto>> AddPatient(
            AddPatientRequestDto request, long actorUserId, UserRole actorRole)
        {
            if (request.DateOfBirth.HasValue && request.DateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
            {
                return Fail("Date of birth cannot be in the future.", HttpStatusCode.BadRequest);
            }

            var defaultPassword = _config["Setup:Owner:DefaultUserPassword"];
            if (string.IsNullOrWhiteSpace(defaultPassword))
            {
                return Fail("Default password is not configured on the server.", HttpStatusCode.InternalServerError);
            }

            // 1. Decide which doctor the patient goes to, based on who is logged in
            var (doctorId, error, status) = await ResolveDoctorAsync(request.DoctorId, actorUserId, actorRole);
            if (error != null)
            {
                return Fail(error, status);
            }

            var doctor = await _patientRepo.GetActiveDoctorAsync(doctorId!.Value);
            if (doctor == null)
            {
                return Fail("The selected doctor is not active.", HttpStatusCode.BadRequest);
            }

            // 2. Patient code prefix from the department and doctor codes, e.g. ORT-SOLUNKE
            if (string.IsNullOrWhiteSpace(doctor.Department.Code) || string.IsNullOrWhiteSpace(doctor.Code))
            {
                return Fail("The doctor or department has no code yet. Please contact the Owner.", HttpStatusCode.BadRequest);
            }
            var codePrefix = $"{doctor.Department.Code}-{doctor.Code}";

            // 3. Optional photo: validate and save before touching the database
            string? photoPath = null;
            if (!string.IsNullOrWhiteSpace(request.PhotoBase64))
            {
                var (path, photoError) = await _fileService.SaveBase64PhotoAsync(request.PhotoBase64, "Patients");
                if (photoError != null)
                {
                    return Fail(photoError, HttpStatusCode.BadRequest);
                }
                photoPath = path;
            }

            // 4. Patient with the default password (stored as a hash) and IsPasswordReset = false
            var patient = new Patient
            {
                FullName = request.FullName.Trim(),
                Mobile = request.Mobile.Trim(),
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                BloodGroup = request.BloodGroup?.Trim().ToUpperInvariant(),
                PhotoPath = photoPath,
                PasswordHash = _securityHelper.HashPassword(defaultPassword),
                IsPasswordReset = false,
                CreatedBy = actorUserId
            };

            try
            {
                await _patientRepo.AddPatientAsync(patient, doctor.Id, actorUserId, codePrefix);
            }
            catch (DbUpdateException)
            {
                _fileService.DeleteFile(photoPath);   // do not leave an orphan photo
                return Fail("Could not save the patient. Please try again.", HttpStatusCode.Conflict);
            }

            return new GeneralResponse<AddPatientResponseDto>
            {
                Success = true,
                Message = "Patient registered. They can log in with the default password and must reset it on first login.",
                HttpStatusCode = HttpStatusCode.Created,
                Data = new AddPatientResponseDto
                {
                    PatientId = patient.Id,
                    PatientCode = patient.PatientCode,
                    FullName = patient.FullName,
                    Mobile = patient.Mobile,
                    DoctorId = doctor.Id,
                    DoctorName = doctor.User.FullName,
                    DepartmentName = doctor.Department.Name,
                    IsPasswordReset = false,
                    PhotoUrl = photoPath == null ? null : $"/v1/api/Patient/{patient.Id}/photo"
                }
            };
        }

        public async Task<string?> GetPatientPhotoFullPathAsync(long patientId)
        {
            var relative = await _patientRepo.GetPatientPhotoPathAsync(patientId);
            var full = _fileService.GetFullPath(relative);
            return full != null && File.Exists(full) ? full : null;
        }

        // Applies the "who can assign to which doctor" rules
        private async Task<(long? DoctorId, string? Error, HttpStatusCode Status)> ResolveDoctorAsync(
            long? requestedDoctorId, long actorUserId, UserRole actorRole)
        {
            var requested = requestedDoctorId is > 0 ? requestedDoctorId : null;

            switch (actorRole)
            {
                case UserRole.Doctor:
                    {
                        var self = await _patientRepo.GetDoctorByUserIdAsync(actorUserId);
                        if (self == null)
                            return (null, "Your doctor profile is not set up. Please contact the Owner.", HttpStatusCode.Forbidden);
                        if (requested.HasValue && requested.Value != self.Id)
                            return (null, "Doctors can only add patients for themselves.", HttpStatusCode.Forbidden);
                        return (self.Id, null, HttpStatusCode.OK);
                    }

                case UserRole.DeptHead:
                    {
                        var self = await _patientRepo.GetDoctorByUserIdAsync(actorUserId);
                        if (self == null)
                            return (null, "Your doctor profile is not set up. Please contact the Owner.", HttpStatusCode.Forbidden);
                        if (!requested.HasValue || requested.Value == self.Id)
                            return (self.Id, null, HttpStatusCode.OK);

                        var target = await _patientRepo.GetActiveDoctorAsync(requested.Value);
                        if (target == null)
                            return (null, $"Doctor with ID {requested} was not found. Use the doctor ID, not the user ID.", HttpStatusCode.BadRequest);
                        if (target.DepartmentId != self.DepartmentId)
                            return (null, "You can only add patients for doctors in your department.", HttpStatusCode.Forbidden);
                        return (target.Id, null, HttpStatusCode.OK);
                    }

                case UserRole.Assistant:
                    {
                        if (!requested.HasValue)
                            return (null, "Select the doctor this patient is for.", HttpStatusCode.BadRequest);
                        if (!await _patientRepo.IsAssistantOfDoctorAsync(actorUserId, requested.Value))
                            return (null, "You can only add patients for the doctors you assist.", HttpStatusCode.Forbidden);
                        return (requested.Value, null, HttpStatusCode.OK);
                    }

                default:
                    return (null, "You are not allowed to add patients.", HttpStatusCode.Forbidden);
            }
        }

        private static GeneralResponse<AddPatientResponseDto> Fail(string message, HttpStatusCode status)
        {
            return new GeneralResponse<AddPatientResponseDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = status,
                Data = null
            };
        }

        public async Task<GeneralResponse<PagedResponseDto<PatientListItemDto>>> GetPatientList(PatientListQueryDto query)
        {
            // Work out which doctors.id to filter by
            long? doctorId = query.DoctorId is > 0 ? query.DoctorId : null;

            if (query.DoctorUserId is > 0)
            {
                doctorId = await _patientRepo.GetDoctorIdByUserIdAsync(query.DoctorUserId.Value);
                if (doctorId == null)
                {
                    return new GeneralResponse<PagedResponseDto<PatientListItemDto>>
                    {
                        Success = false,
                        Message = $"User {query.DoctorUserId} has no doctor profile.",
                        HttpStatusCode = HttpStatusCode.NotFound
                    };
                }
            }

            var (items, total) = await _patientRepo.GetPatientListAsync(query, doctorId);

            foreach (var p in items)
            {
                p.GenderName = PersonDisplay.GenderName(p.Gender);
                p.Age = PersonDisplay.Age(p.DateOfBirth);
                p.PhotoUrl = PersonDisplay.PatientPhotoUrl(p.PatientId, p.PhotoPath);
            }

            return new GeneralResponse<PagedResponseDto<PatientListItemDto>>
            {
                Success = true,
                Message = items.Count == 0 ? "No patients found." : "Success",
                HttpStatusCode = HttpStatusCode.OK,
                Data = new PagedResponseDto<PatientListItemDto>
                {
                    Items = items,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalCount = total
                }
            };
        }

        public async Task<GeneralResponse<PatientDetailDto>> GetPatientDetail(long patientId)
        {
            var patient = await _patientRepo.GetPatientDetailAsync(patientId);
            if (patient == null)
            {
                return new GeneralResponse<PatientDetailDto>
                {
                    Success = false,
                    Message = "Patient not found.",
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }

            patient.GenderName = PersonDisplay.GenderName(patient.Gender);
            patient.Age = PersonDisplay.Age(patient.DateOfBirth);
            patient.PhotoUrl = PersonDisplay.PatientPhotoUrl(patient.PatientId, patient.PhotoPath);

            return new GeneralResponse<PatientDetailDto>
            {
                Success = true,
                Message = "Success",
                HttpStatusCode = HttpStatusCode.OK,
                Data = patient
            };
        }
    }
}
