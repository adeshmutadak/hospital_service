using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AddDepartmentRequestDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        // Short code used in patient codes, e.g. ORT, GYN, DER
        [Required, RegularExpression(@"^[A-Za-z]{2,5}$", ErrorMessage = "Code must be 2 to 5 letters, e.g. ORT.")]
        public string Code { get; set; } = null!;
    }
}
