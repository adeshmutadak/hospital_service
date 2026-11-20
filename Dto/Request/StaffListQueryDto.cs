using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class StaffListQueryDto
    {
        // 3 = DeptHead, 4 = Doctor, 5 = Assistant
        [Range(3, 5, ErrorMessage = "Role must be 3 (DeptHead), 4 (Doctor) or 5 (Assistant).")]
        public byte Role { get; set; }

        public string? Search { get; set; }          // name, mobile or doctor code
        public long? DepartmentId { get; set; }
        public bool? IsActive { get; set; } = true;  // null = active and inactive

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
    }
}
