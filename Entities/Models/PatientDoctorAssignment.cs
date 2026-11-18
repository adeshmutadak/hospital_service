using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class PatientDoctorAssignment
{
    public long Id { get; set; }

    public long PatientId { get; set; }

    public long DoctorId { get; set; }

    public bool? IsPrimary { get; set; }

    public long AssignedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual User AssignedByNavigation { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
