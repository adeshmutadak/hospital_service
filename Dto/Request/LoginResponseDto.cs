using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class LoginResponseDto
    {
        public long UserId { get; set; }
        public string? HospitalName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public byte Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public long? DoctorId { get; set; }
        public long? DepartmentId { get; set; }
        public long? PatientId { get; set; }
        public string? PhotoPath { get; set; }
        public bool MustChangePassword { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
