using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AddUserResponseDto
    {
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public byte Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool MustChangePassword { get; set; }
    }
}
