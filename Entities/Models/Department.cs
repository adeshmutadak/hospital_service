using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class Department
{
    public long Id { get; set; }


    public string Name { get; set; } = null!;

    public long? HeadDoctorId { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public string? Code { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual Doctor? HeadDoctor { get; set; }
}
