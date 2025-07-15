using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class user
{
    public int id { get; set; }

    public string? first_name { get; set; }

    public string? last_name { get; set; }

    public string? user_name { get; set; }

    public string email { get; set; } = null!;

    public string? phone_number { get; set; }

    public int? class_id { get; set; }

    public string? avatar_path { get; set; }

    public string? gender { get; set; }

    public string? role { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }
}
