using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class DoctorSchedule
{
    public long Id { get; set; }

    public long DoctorId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;
}
