using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class UserTask
{
    public int id { get; set; }

    public int class_id { get; set; }

    public string task_name { get; set; } = null!;

    public DateTime start_time { get; set; }

    public DateTime end_time { get; set; }

    public string? description { get; set; }

    public string? status { get; set; }

    public string? file { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }
}
