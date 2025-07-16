using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Notification
{
    public int id { get; set; }

    public int? actor_id { get; set; }

    public string? notification_type { get; set; }

    public string? content { get; set; }

    public string? url { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public virtual ICollection<Notification_recipient> Notification_recipients { get; set; } = new List<Notification_recipient>();
}
