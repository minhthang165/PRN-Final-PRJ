using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Class
{
    public int id { get; set; }

    public string class_name { get; set; } = null!;

    public string? class_description { get; set; }

    public int? number_of_interns { get; set; }

    public string? status { get; set; }

    public int? mentor_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public int? conversation_id { get; set; }

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();

    public virtual user mentor { get; set; } = null!;
}
