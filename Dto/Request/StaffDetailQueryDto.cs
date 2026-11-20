using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class StaffDetailQueryDto
    {
        // 3 = DeptHead, 4 = Doctor, 5 = Assistant
        [Range(3, 5, ErrorMessage = "Role must be 3 (DeptHead), 4 (Doctor) or 5 (Assistant).")]
        public byte Role { get; set; }

        // users.id
        [Range(1, long.MaxValue, ErrorMessage = "Id must be a valid user ID.")]
        public long Id { get; set; }
    }
}
