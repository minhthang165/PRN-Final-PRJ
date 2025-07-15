using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Room
{
    public int id { get; set; }

    public string room_name { get; set; } = null!;

    public string? location { get; set; }

    public int? capicity { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }
}
