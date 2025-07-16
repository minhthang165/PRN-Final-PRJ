using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class CV_Info
{
    public int cvInfo_id { get; set; }

    public int recruitment_id { get; set; }

    public int file_id { get; set; }

    public decimal? gpa { get; set; }

    public string education { get; set; } = null!;

    public string skill { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public virtual UserFile file { get; set; } = null!;

    public virtual Recruitment recruitment { get; set; } = null!;
}
