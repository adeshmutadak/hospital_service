using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enum
{
    public enum UserRole : byte
    {
        PlatformAdmin = 1,
        Owner = 2,
        DeptHead = 3,
        Doctor = 4,
        Assistant = 5,
        Patient = 6
    }

}
