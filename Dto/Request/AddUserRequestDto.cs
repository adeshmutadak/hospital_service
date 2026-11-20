using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AddUserRequestDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        public byte Role { get; set; }
    }
}
