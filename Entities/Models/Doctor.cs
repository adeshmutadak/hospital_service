using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class Doctor
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long DepartmentId { get; set; }

    public long? ReportsToDoctorId { get; set; }

    public string? Qualification { get; set; }

    public string? Specialization { get; set; }

    public string? RegistrationNo { get; set; }

    public decimal? ConsultationFee { get; set; }

    public int SlotMinutes { get; set; }
    public string? Code { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public virtual ICollection<AssistantAssignment> AssistantAssignments { get; set; } = new List<AssistantAssignment>();

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    public virtual ICollection<Doctor> InverseReportsToDoctor { get; set; } = new List<Doctor>();

    public virtual ICollection<PatientDoctorAssignment> PatientDoctorAssignments { get; set; } = new List<PatientDoctorAssignment>();

    public virtual Doctor? ReportsToDoctor { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
