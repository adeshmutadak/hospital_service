using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto.Request;
using Hospital_Service.Data;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class PatientRepo : IPatientRepo
    {
        private readonly HospitalDbContext _context;

        public PatientRepo(HospitalDbContext context)
        {
            _context = context;
        }

        // Includes User (doctor name) and Department (name and code)
        public async Task<Doctor?> GetActiveDoctorAsync(long doctorId)
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == doctorId && !d.IsDeleted
                                          && !d.User.IsDeleted && d.User.IsActive != false);
        }

        // Logged-in doctor or department head's own doctor profile
        public async Task<Doctor?> GetDoctorByUserIdAsync(long userId)
        {
            return await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.UserId == userId && !d.IsDeleted);
        }

        public async Task<bool> IsAssistantOfDoctorAsync(long assistantUserId, long doctorId)
        {
            return await _context.AssistantAssignments
                .AnyAsync(a => a.AssistantUserId == assistantUserId && a.DoctorId == doctorId && !a.IsDeleted);
        }

        public async Task AddPatientAsync(Patient patient, long doctorId, long actorUserId, string codePrefix)
        {
            // EnableRetryOnFailure requires transactions to run inside the execution strategy
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                patient.PatientDoctorAssignments.Add(new PatientDoctorAssignment
                {
                    DoctorId = doctorId,
                    IsPrimary = true,
                    AssignedBy = actorUserId
                });

                // Current main doctor
                patient.DoctorId = doctorId;

                // Temporary unique code (20 characters), replaced once the ID is known
                patient.PatientCode = "TMP" + Guid.NewGuid().ToString("N")[..17];
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                // e.g. ORT-SOLUNKE-0042
                patient.PatientCode = $"{codePrefix}-{patient.Id:D4}";
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            });
        }

        public async Task<string?> GetPatientPhotoPathAsync(long patientId)
        {
            return await _context.Patients
                .AsNoTracking()
                .Where(p => p.Id == patientId && !p.IsDeleted)
                .Select(p => p.PhotoPath)
                .FirstOrDefaultAsync();
        }
        public async Task<long?> GetDoctorIdByUserIdAsync(long userId)
        {
            return await _context.Doctors.AsNoTracking()
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .Select(d => (long?)d.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<PatientListItemDto> Items, int Total)> GetPatientListAsync(PatientListQueryDto q, long? doctorId)
        {
            var query = _context.Patients.AsNoTracking().Where(p => !p.IsDeleted);

            if (doctorId.HasValue)
                query = query.Where(p => p.DoctorId == doctorId.Value);

            if (q.DepartmentId.HasValue)
                query = query.Where(p => p.Doctor != null && p.Doctor.DepartmentId == q.DepartmentId.Value);

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                query = query.Where(p => p.FullName.Contains(s) || p.Mobile.Contains(s) || p.PatientCode.Contains(s));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(p => new PatientListItemDto
                {
                    PatientId = p.Id,
                    PatientCode = p.PatientCode,
                    FullName = p.FullName,
                    Mobile = p.Mobile,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    BloodGroup = p.BloodGroup,
                    PhotoPath = p.PhotoPath,
                    DoctorId = p.DoctorId,
                    DoctorName = p.Doctor != null ? p.Doctor.User.FullName : null,
                    DoctorCode = p.Doctor != null ? p.Doctor.Code : null,
                    DepartmentName = p.Doctor != null ? p.Doctor.Department.Name : null,
                    LastVisitDate = p.Visits.Where(v => !v.IsDeleted).Max(v => (DateTime?)v.VisitDate),
                    NextVisitDate = p.Visits.Where(v => !v.IsDeleted)
                        .OrderByDescending(v => v.VisitDate)
                        .Select(v => v.NextVisitDate)
                        .FirstOrDefault(),
                    IsPasswordReset = p.IsPasswordReset,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return (items, total);
        }
        public async Task<PatientDetailDto?> GetPatientDetailAsync(long patientId)
        {
            return await _context.Patients.AsNoTracking()
                .Where(p => p.Id == patientId && !p.IsDeleted)
                .Select(p => new PatientDetailDto
                {
                    PatientId = p.Id,
                    PatientCode = p.PatientCode,
                    FullName = p.FullName,
                    Mobile = p.Mobile,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    BloodGroup = p.BloodGroup,
                    PhotoPath = p.PhotoPath,
                    IsPasswordReset = p.IsPasswordReset,
                    CreatedAt = p.CreatedAt,
                    RegisteredBy = p.CreatedByNavigation.FullName,
                    DoctorId = p.DoctorId,
                    DoctorUserId = p.Doctor != null ? p.Doctor.UserId : null,
                    DoctorName = p.Doctor != null ? p.Doctor.User.FullName : null,
                    DoctorCode = p.Doctor != null ? p.Doctor.Code : null,
                    DepartmentName = p.Doctor != null ? p.Doctor.Department.Name : null,
                    VisitCount = p.Visits.Count(v => !v.IsDeleted),
                    LastVisitDate = p.Visits.Where(v => !v.IsDeleted).Max(v => (DateTime?)v.VisitDate),
                    NextVisitDate = p.Visits.Where(v => !v.IsDeleted)
                        .OrderByDescending(v => v.VisitDate)
                        .Select(v => v.NextVisitDate)
                        .FirstOrDefault(),
                    AppointmentCount = p.Appointments.Count(a => !a.IsDeleted)
                })
                .FirstOrDefaultAsync();
        }
    }
}
