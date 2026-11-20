using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class DepartmentResponseDto
    {
        public long DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public long? HeadDoctorId { get; set; }
        public bool IsActive { get; set; }
    }
}
