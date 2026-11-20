using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class StaffDetailDto
    {
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public byte Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime CreatedAt { get; set; }

        // Heads and doctors
        public DoctorProfileDto? DoctorProfile { get; set; }

        // Assistants
        public List<PersonSummaryDto>? AssistsDoctors { get; set; }
    }

    public class DoctorProfileDto
    {
        public long DoctorId { get; set; }
        public string? Code { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; }
        public bool IsDepartmentHead { get; set; }
        public long? ReportsToDoctorId { get; set; }
        public string? ReportsToName { get; set; }
        public string? Qualification { get; set; }
        public string? Specialization { get; set; }
        public string? RegistrationNo { get; set; }
        public decimal? ConsultationFee { get; set; }
        public int SlotMinutes { get; set; }
        public int PatientCount { get; set; }
        public List<PersonSummaryDto> Assistants { get; set; } = new();
        public List<PersonSummaryDto> JuniorDoctors { get; set; } = new();
    }

    public class PersonSummaryDto
    {
        public long UserId { get; set; }
        public long? DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Code { get; set; }
    }
}
