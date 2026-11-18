using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class AssistantAssignment
{
    public long Id { get; set; }

    public long HospitalId { get; set; }

    public long AssistantUserId { get; set; }

    public long DoctorId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual User AssistantUser { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Hospital Hospital { get; set; } = null!;
}
