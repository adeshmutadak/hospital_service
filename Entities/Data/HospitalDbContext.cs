using System;
using System.Collections.Generic;
using Hospital_Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Service.Data;

public partial class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AssistantAssignment> AssistantAssignments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<MedicalReport> MedicalReports { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientDoctorAssignment> PatientDoctorAssignments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("appointments");

            entity.HasIndex(e => e.CreatedBy, "fk_appt_created_by");

            entity.HasIndex(e => e.DoctorId, "fk_appt_doctor");

            entity.HasIndex(e => e.HospitalId, "fk_appt_hospital");

            entity.HasIndex(e => e.PatientId, "fk_appt_patient");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_appt_created_by");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appt_doctor");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appt_hospital");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appt_patient");
        });

        modelBuilder.Entity<AssistantAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("assistant_assignments");

            entity.HasIndex(e => e.DoctorId, "fk_assist_doctor");

            entity.HasIndex(e => e.HospitalId, "fk_assist_hospital");

            entity.HasIndex(e => e.AssistantUserId, "fk_assist_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssistantUserId).HasColumnName("assistant_user_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            entity.HasOne(d => d.AssistantUser).WithMany(p => p.AssistantAssignments)
                .HasForeignKey(d => d.AssistantUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_assist_user");

            entity.HasOne(d => d.Doctor).WithMany(p => p.AssistantAssignments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_assist_doctor");

            entity.HasOne(d => d.Hospital).WithMany(p => p.AssistantAssignments)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_assist_hospital");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departments");

            entity.HasIndex(e => e.HeadDoctorId, "fk_departments_head_doctor");

            entity.HasIndex(e => new { e.HospitalId, e.Name }, "uq_departments_hospital_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.HeadDoctorId).HasColumnName("head_doctor_id");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.HeadDoctor).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HeadDoctorId)
                .HasConstraintName("fk_departments_head_doctor");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_departments_hospital");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doctors");

            entity.HasIndex(e => e.DepartmentId, "fk_doctors_department");

            entity.HasIndex(e => e.HospitalId, "fk_doctors_hospital");

            entity.HasIndex(e => e.ReportsToDoctorId, "fk_doctors_reports_to");

            entity.HasIndex(e => e.UserId, "uq_doctors_user").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConsultationFee)
                .HasPrecision(10, 2)
                .HasColumnName("consultation_fee");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Qualification)
                .HasMaxLength(150)
                .HasColumnName("qualification");
            entity.Property(e => e.RegistrationNo)
                .HasMaxLength(50)
                .HasColumnName("registration_no");
            entity.Property(e => e.ReportsToDoctorId).HasColumnName("reports_to_doctor_id");
            entity.Property(e => e.SlotMinutes)
                .HasDefaultValueSql("'15'")
                .HasColumnName("slot_minutes");
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .HasColumnName("specialization");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Department).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_doctors_department");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_doctors_hospital");

            entity.HasOne(d => d.ReportsToDoctor).WithMany(p => p.InverseReportsToDoctor)
                .HasForeignKey(d => d.ReportsToDoctorId)
                .HasConstraintName("fk_doctors_reports_to");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_doctors_user");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doctor_schedules");

            entity.HasIndex(e => e.DoctorId, "fk_schedules_doctor");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("time")
                .HasColumnName("end_time");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.StartTime)
                .HasColumnType("time")
                .HasColumnName("start_time");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSchedules)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedules_doctor");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("hospitals");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<MedicalReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("medical_reports");

            entity.HasIndex(e => e.HospitalId, "fk_reports_hospital");

            entity.HasIndex(e => e.PatientId, "fk_reports_patient");

            entity.HasIndex(e => e.UploadedBy, "fk_reports_uploaded_by");

            entity.HasIndex(e => e.VisitId, "fk_reports_visit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileName)
                .HasMaxLength(200)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(300)
                .HasColumnName("file_path");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ReportType).HasColumnName("report_type");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");
            entity.Property(e => e.VisitId).HasColumnName("visit_id");

            entity.HasOne(d => d.Hospital).WithMany(p => p.MedicalReports)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reports_hospital");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalReports)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reports_patient");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.MedicalReports)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reports_uploaded_by");

            entity.HasOne(d => d.Visit).WithMany(p => p.MedicalReports)
                .HasForeignKey(d => d.VisitId)
                .HasConstraintName("fk_reports_visit");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("patients");

            entity.HasIndex(e => e.CreatedBy, "fk_patients_created_by");

            entity.HasIndex(e => new { e.HospitalId, e.PatientCode }, "uq_patients_hospital_code").IsUnique();

            entity.HasIndex(e => e.UserId, "uq_patients_user").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(5)
                .HasColumnName("blood_group");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .HasColumnName("mobile");
            entity.Property(e => e.PatientCode)
                .HasMaxLength(20)
                .HasColumnName("patient_code");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PatientCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patients_created_by");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Patients)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patients_hospital");

            entity.HasOne(d => d.User).WithOne(p => p.PatientUser)
                .HasForeignKey<Patient>(d => d.UserId)
                .HasConstraintName("fk_patients_user");
        });

        modelBuilder.Entity<PatientDoctorAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("patient_doctor_assignments");

            entity.HasIndex(e => e.AssignedBy, "fk_pda_assigned_by");

            entity.HasIndex(e => e.DoctorId, "fk_pda_doctor");

            entity.HasIndex(e => e.PatientId, "fk_pda_patient");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsPrimary)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_primary");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.PatientDoctorAssignments)
                .HasForeignKey(d => d.AssignedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pda_assigned_by");

            entity.HasOne(d => d.Doctor).WithMany(p => p.PatientDoctorAssignments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pda_doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientDoctorAssignments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pda_patient");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => new { e.HospitalId, e.Mobile }, "uq_users_hospital_mobile").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .HasColumnName("full_name");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .HasColumnName("mobile");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.MustChangePassword)
                .HasDefaultValueSql("'0'")
                .HasColumnName("must_change_password");
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(300)
                .HasColumnName("photo_path");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Users)
                .HasForeignKey(d => d.HospitalId)
                .HasConstraintName("fk_users_hospital");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("visits");

            entity.HasIndex(e => e.AppointmentId, "fk_visits_appointment");

            entity.HasIndex(e => e.DoctorId, "fk_visits_doctor");

            entity.HasIndex(e => e.HospitalId, "fk_visits_hospital");

            entity.HasIndex(e => e.PatientId, "fk_visits_patient");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.Complaints)
                .HasColumnType("text")
                .HasColumnName("complaints");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Diagnosis)
                .HasColumnType("text")
                .HasColumnName("diagnosis");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.HospitalId).HasColumnName("hospital_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.NextVisitDate).HasColumnName("next_visit_date");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Prescription)
                .HasColumnType("text")
                .HasColumnName("prescription");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VisibleToPatient)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("visible_to_patient");
            entity.Property(e => e.VisitDate)
                .HasColumnType("datetime")
                .HasColumnName("visit_date");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Visits)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("fk_visits_appointment");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Visits)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_visits_doctor");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Visits)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_visits_hospital");

            entity.HasOne(d => d.Patient).WithMany(p => p.Visits)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_visits_patient");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
