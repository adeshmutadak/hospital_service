using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class StaffListItemDto
    {
        public long UserId { get; set; }            // use for GET /User/detail
        public long? DoctorId { get; set; }         // doctors.id (heads and doctors); use for patients
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public byte Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Code { get; set; }           // doctor code, e.g. SOLUNKE
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsDepartmentHead { get; set; }
        public int? PatientCount { get; set; }      // heads and doctors
        public int? AssistsDoctorCount { get; set; } // assistants
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
