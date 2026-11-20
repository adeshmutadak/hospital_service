using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AddDoctorRequestDto
    {
        // User created earlier by POST /v1/api/User/add with role 3 (DeptHead) or 4 (Doctor)
        [Range(1, long.MaxValue, ErrorMessage = "Select a user.")]
        public long UserId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Select a department.")]
        public long DepartmentId { get; set; }

        // Short code used in patient codes, e.g. SOLUNKE or PDEKATE
        [Required, RegularExpression(@"^[A-Za-z]{2,10}$", ErrorMessage = "Code must be 2 to 10 letters, e.g. SOLUNKE.")]
        public string Code { get; set; } = null!;

        public long? ReportsToDoctorId { get; set; }

        [MaxLength(150)]
        public string? Qualification { get; set; }

        [MaxLength(100)]
        public string? Specialization { get; set; }

        [MaxLength(50)]
        public string? RegistrationNo { get; set; }

        [Range(0, 100000)]
        public decimal? ConsultationFee { get; set; }

        [Range(5, 120)]
        public int SlotMinutes { get; set; } = 15;
    }
}
