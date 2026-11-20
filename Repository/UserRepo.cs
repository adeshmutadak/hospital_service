using Dto.Request;
using Entities.Models.Enum;
using Hospital_Service.Data;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly HospitalDbContext _context;

        public UserRepo(HospitalDbContext context)
        {
            _context = context;
        }

        // Checks deleted users too, because the unique key on mobile covers every row
        public async Task<bool> MobileExistsAsync(string mobile)
        {
            return await _context.Users.AnyAsync(u => u.Mobile == mobile);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }



        public async Task<(List<StaffListItemDto> Items, int Total)> GetStaffListAsync(StaffListQueryDto q)
        {
            var role = (UserRole)q.Role;

            var query = _context.Users.AsNoTracking()
                .Where(u => !u.IsDeleted && u.Role == role);

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                query = query.Where(u => u.FullName.Contains(s)
                                      || u.Mobile.Contains(s)
                                      || (u.Doctor != null && u.Doctor.Code != null && u.Doctor.Code.Contains(s)));
            }

            if (q.IsActive.HasValue)
            {
                query = query.Where(u => (u.IsActive ?? true) == q.IsActive.Value);
            }

            if (q.DepartmentId.HasValue)
            {
                // Heads and doctors by their own department; assistants by their doctors' department
                query = query.Where(u =>
                    (u.Doctor != null && !u.Doctor.IsDeleted && u.Doctor.DepartmentId == q.DepartmentId.Value) ||
                    u.AssistantAssignments.Any(a => !a.IsDeleted && a.Doctor.DepartmentId == q.DepartmentId.Value));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.FullName)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(u => new StaffListItemDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Mobile = u.Mobile,
                    Email = u.Email,
                    Role = (byte)u.Role,
                    IsActive = u.IsActive ?? true,
                    MustChangePassword = u.MustChangePassword,
                    CreatedAt = u.CreatedAt,

                    // Heads and doctors (null if no doctor profile yet)
                    DoctorId = u.Doctor != null && !u.Doctor.IsDeleted ? u.Doctor.Id : null,
                    Code = u.Doctor != null && !u.Doctor.IsDeleted ? u.Doctor.Code : null,
                    DepartmentId = u.Doctor != null && !u.Doctor.IsDeleted ? u.Doctor.DepartmentId : null,
                    DepartmentName = u.Doctor != null && !u.Doctor.IsDeleted ? u.Doctor.Department.Name : null,
                    IsDepartmentHead = u.Doctor != null && u.Doctor.Departments.Any(d => !d.IsDeleted),
                    PatientCount = u.Doctor != null && !u.Doctor.IsDeleted
                        ? u.Doctor.Patients.Count(p => !p.IsDeleted)
                        : null,

                    // Assistants
                    AssistsDoctorCount = u.Role == UserRole.Assistant
                        ? u.AssistantAssignments.Count(a => !a.IsDeleted && !a.Doctor.IsDeleted)
                        : null
                })
                .ToListAsync();

            return (items, total);
        }


        public async Task<StaffDetailDto?> GetStaffDetailAsync(long userId)
        {
            return await _context.Users.AsNoTracking()
                .Where(u => u.Id == userId && !u.IsDeleted)
                .Select(u => new StaffDetailDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Mobile = u.Mobile,
                    Email = u.Email,
                    Role = (byte)u.Role,
                    IsActive = u.IsActive ?? true,
                    MustChangePassword = u.MustChangePassword,
                    CreatedAt = u.CreatedAt,

                    DoctorProfile = u.Doctor == null || u.Doctor.IsDeleted ? null : new DoctorProfileDto
                    {
                        DoctorId = u.Doctor.Id,
                        Code = u.Doctor.Code,
                        DepartmentId = u.Doctor.DepartmentId,
                        DepartmentName = u.Doctor.Department.Name,
                        DepartmentCode = u.Doctor.Department.Code,
                        IsDepartmentHead = u.Doctor.Departments.Any(d => !d.IsDeleted),
                        ReportsToDoctorId = u.Doctor.ReportsToDoctorId,
                        ReportsToName = u.Doctor.ReportsToDoctor != null ? u.Doctor.ReportsToDoctor.User.FullName : null,
                        Qualification = u.Doctor.Qualification,
                        Specialization = u.Doctor.Specialization,
                        RegistrationNo = u.Doctor.RegistrationNo,
                        ConsultationFee = u.Doctor.ConsultationFee,
                        SlotMinutes = u.Doctor.SlotMinutes,
                        PatientCount = u.Doctor.Patients.Count(p => !p.IsDeleted),
                        Assistants = u.Doctor.AssistantAssignments
                            .Where(a => !a.IsDeleted && !a.AssistantUser.IsDeleted)
                            .Select(a => new PersonSummaryDto
                            {
                                UserId = a.AssistantUserId,
                                FullName = a.AssistantUser.FullName,
                                Mobile = a.AssistantUser.Mobile
                            }).ToList(),
                        JuniorDoctors = u.Doctor.InverseReportsToDoctor
                            .Where(j => !j.IsDeleted && !j.User.IsDeleted)
                            .Select(j => new PersonSummaryDto
                            {
                                UserId = j.UserId,
                                DoctorId = j.Id,
                                FullName = j.User.FullName,
                                Mobile = j.User.Mobile,
                                Code = j.Code
                            }).ToList()
                    },

                    AssistsDoctors = u.Role == UserRole.Assistant
                        ? u.AssistantAssignments
                            .Where(a => !a.IsDeleted && !a.Doctor.IsDeleted)
                            .Select(a => new PersonSummaryDto
                            {
                                UserId = a.Doctor.UserId,
                                DoctorId = a.DoctorId,
                                FullName = a.Doctor.User.FullName,
                                Mobile = a.Doctor.User.Mobile,
                                Code = a.Doctor.Code
                            }).ToList()
                        : null
                })
                .FirstOrDefaultAsync();
        }
    }
}