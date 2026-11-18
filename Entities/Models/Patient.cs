using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class Patient
{
    public long Id { get; set; }

    public long HospitalId { get; set; }

    public long? UserId { get; set; }

    public string PatientCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public byte? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? BloodGroup { get; set; }

    public long CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Hospital Hospital { get; set; } = null!;

    public virtual ICollection<MedicalReport> MedicalReports { get; set; } = new List<MedicalReport>();

    public virtual ICollection<PatientDoctorAssignment> PatientDoctorAssignments { get; set; } = new List<PatientDoctorAssignment>();

    public virtual User? User { get; set; }

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
