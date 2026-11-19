using System;
using System.Collections.Generic;
using Entities.Models.Enum;

namespace Hospital_Service.Models;

public partial class User
{
    public long Id { get; set; }

    public long? HospitalId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string Mobile { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool MustChangePassword { get; set; }
    public UserRole Role { get; set; }

    public string? PhotoPath { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AssistantAssignment> AssistantAssignments { get; set; } = new List<AssistantAssignment>();

    public virtual Doctor? Doctor { get; set; }

    public virtual Hospital? Hospital { get; set; }

    public virtual ICollection<MedicalReport> MedicalReports { get; set; } = new List<MedicalReport>();

    public virtual ICollection<Patient> PatientCreatedByNavigations { get; set; } = new List<Patient>();

    public virtual ICollection<PatientDoctorAssignment> PatientDoctorAssignments { get; set; } = new List<PatientDoctorAssignment>();

    public virtual Patient? PatientUser { get; set; }
}
