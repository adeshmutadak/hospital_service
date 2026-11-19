using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class LoginRequestDto
    {
        [Required, MaxLength(150)]
        public string MobileOrEmail { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Password { get; set; } = null!;
    }
}
