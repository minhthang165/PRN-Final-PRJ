using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Schedule
{
    public int id { get; set; }

    public int class_id { get; set; }

    public int subject_id { get; set; }

    public int room_id { get; set; }

    public string day_of_week { get; set; } = null!;

    public TimeOnly start_time { get; set; }

    public TimeOnly end_time { get; set; }

    public DateOnly start_date { get; set; }

    public DateOnly end_date { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public int? mentor_id { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Class _class { get; set; } = null!;

    public virtual user? mentor { get; set; }

    public virtual Room room { get; set; } = null!;

    public virtual Subject subject { get; set; } = null!;
}
