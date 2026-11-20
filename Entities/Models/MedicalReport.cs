using System;
using System.Collections.Generic;

namespace Hospital_Service.Models;

public partial class MedicalReport
{
    public long Id { get; set; }


    public long PatientId { get; set; }

    public long? VisitId { get; set; }

    public byte ReportType { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public long UploadedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual User UploadedByNavigation { get; set; } = null!;

    public virtual Visit? Visit { get; set; }
}
