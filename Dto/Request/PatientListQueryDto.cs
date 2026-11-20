using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class PatientListQueryDto
    {
        // Use ONE of these to filter by doctor. doctorUserId is users.id, doctorId is doctors.id
        public long? DoctorUserId { get; set; }
        public long? DoctorId { get; set; }

        public long? DepartmentId { get; set; }
        public string? Search { get; set; }   // name, mobile or patient code

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
    }
}
