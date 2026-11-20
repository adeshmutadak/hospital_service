using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class Appointment
{
    public long Id { get; set; }

    public long PatientId { get; set; }

    public long DoctorId { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public byte Status { get; set; }

    public byte Source { get; set; }

    public string? Reason { get; set; }

    public long? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
