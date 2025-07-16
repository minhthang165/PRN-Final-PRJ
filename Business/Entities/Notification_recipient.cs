using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Notification_recipient
{
    public int id { get; set; }

    public int? notification_id { get; set; }

    public int? recipient_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public virtual Notification? notification { get; set; }

    public virtual user? recipient { get; set; }
}
