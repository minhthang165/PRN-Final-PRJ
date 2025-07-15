using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Recruitment
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string position { get; set; } = null!;

    public string experience_requirement { get; set; } = null!;

    public string? language { get; set; }

    public decimal? min_GPA { get; set; }

    public int total_slot { get; set; }

    public string? description { get; set; }

    public DateTime end_time { get; set; }

    public int class_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }
}
