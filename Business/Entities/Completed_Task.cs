using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Completed_Task
{
    public int task_id { get; set; }

    public int user_id { get; set; }

    public int class_id { get; set; }

    public string? file { get; set; }

    public string? status { get; set; }

    public int? mark { get; set; }

    public string? comment { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }
}
