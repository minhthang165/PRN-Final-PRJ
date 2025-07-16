using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Subject
{
    public int id { get; set; }

    public string subject_name { get; set; } = null!;

    public string? description { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
