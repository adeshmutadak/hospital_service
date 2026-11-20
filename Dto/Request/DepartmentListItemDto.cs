using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class DepartmentListItemDto
    {
        public long DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public long? HeadDoctorId { get; set; }
        public string? HeadDoctorName { get; set; }
        public int DoctorCount { get; set; }
        public bool IsActive { get; set; }
    }
}
