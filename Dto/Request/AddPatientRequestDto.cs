using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AddPatientRequestDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = null!;

        [Required, RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string Mobile { get; set; } = null!;

        // 1 = Female, 2 = Male, 3 = Other
        [Range(1, 3, ErrorMessage = "Gender must be 1 (Female), 2 (Male) or 3 (Other).")]
        public byte? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [RegularExpression(@"^(A|B|AB|O)[+-]$", ErrorMessage = "Blood group must be like A+, B-, AB+ or O-.")]
        public string? BloodGroup { get; set; }

        // Doctor to assign (doctors.id, not users.id).
        // Doctor: ignored, always themselves. DeptHead: optional, defaults to themselves.
        // Assistant: required, must be a doctor they assist.
        public long? DoctorId { get; set; }

        // Optional base64 JPG or PNG, with or without the "data:image/...;base64," prefix (max 2 MB image)
        [MaxLength(3_000_000, ErrorMessage = "Photo is too large.")]
        public string? PhotoBase64 { get; set; }
    }
}
