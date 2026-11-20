using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class Patient
{
    public long Id { get; set; }

    public long? DoctorId { get; set; }

    public string PatientCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public byte? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? BloodGroup { get; set; }

    public string? PhotoPath { get; set; }

    public string? PasswordHash { get; set; }

    public bool IsPasswordReset { get; set; }

    public long CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Doctor? Doctor { get; set; }

    public virtual ICollection<MedicalReport> MedicalReports { get; set; } = new List<MedicalReport>();

    public virtual ICollection<PatientDoctorAssignment> PatientDoctorAssignments { get; set; } = new List<PatientDoctorAssignment>();

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}