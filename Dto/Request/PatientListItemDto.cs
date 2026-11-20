using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class PatientListItemDto
    {
        public long PatientId { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public byte? Gender { get; set; }
        public string? GenderName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? BloodGroup { get; set; }
        public string? PhotoUrl { get; set; }
        public long? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorCode { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public DateOnly? NextVisitDate { get; set; }
        public bool IsPasswordReset { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public string? PhotoPath { get; set; }
    }
}
