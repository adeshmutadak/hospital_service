using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class Visit
{
    public long Id { get; set; }

    public long HospitalId { get; set; }

    public long PatientId { get; set; }

    public long DoctorId { get; set; }

    public long? AppointmentId { get; set; }

    public DateTime VisitDate { get; set; }

    public string? Complaints { get; set; }

    public string? Diagnosis { get; set; }

    public string? Notes { get; set; }

    public string? Prescription { get; set; }

    public DateOnly? NextVisitDate { get; set; }

    public bool? VisibleToPatient { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Hospital Hospital { get; set; } = null!;

    public virtual ICollection<MedicalReport> MedicalReports { get; set; } = new List<MedicalReport>();

    public virtual Patient Patient { get; set; } = null!;
}
